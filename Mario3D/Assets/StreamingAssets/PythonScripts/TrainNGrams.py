#!/usr/bin/env python
# coding: utf-8


# -----------------------------------------------------------
# 
#
# 2020 
# Víctor Emiliano Fernández Rubio
# Gonzalo Guzmán del Río
# Carlos Llames Arribas
# 
# -----------------------------------------------------------

#EXAMPLE COMMAND:
#
# One file without interpolation
# python TrainNGrams.py 1 1-1.csv 3 0(-d/--debug)
#
# One file with interpolation
# python TrainNGrams.py 1 1-1.csv 3 1 0.7 0.2 0.1(-d/--debug)
#
# Several files without interpolation
# python TrainNGrams.py 2 1-1.csv 1-2.csv 0(-d/--debug)
#
# Several files with interpolation
# python TrainNGrams.py 2 1-1.csv 1-2.csv 3 1 0.7 0.2 0.1(-d/--debug)


from nltk.util import bigrams
from nltk.util import ngrams
from nltk import word_tokenize
import numpy as np

import sys
import argparse
import random
import os
import csv
import time
import pickle

# Read arguments
NFILES = int(sys.argv[1])
FILE = []
for f in range(NFILES):
    FILE.append(str(sys.argv[f + 2]))

N = int(sys.argv[NFILES + 2])

INTERPOLATION = int(sys.argv[NFILES + 3])
LAMBDAPERCENT = []
if(INTERPOLATION == 1):
    for percent in range(N):
        LAMBDAPERCENT.append(float(sys.argv[NFILES + percent + 4]))

DEPURATION = False
if(len(sys.argv) > NFILES + len(LAMBDAPERCENT) + 4):
    if str(sys.argv[NFILES + len(LAMBDAPERCENT) + 4]) == "-d" or str(sys.argv[NFILES + len(LAMBDAPERCENT) + 4]) == "--debug":
        DEPURATION = True
        try: 
            path = "../Logs/"
            if not os.path.exists(path):
                os.makedirs(path)
            logFileName = time.strftime("%Y%m%d-%H%M%S")
            sys.stdout = open(path + logFileName +"_Training_Ngrams.txt", "w")
        except OSError:
            if not os.path.isdir(path):
                raise


# Read a file and is delimited by ','
def ReadFile(file):
    
    text = np.genfromtxt(file, delimiter=',',dtype=None)
    text = text.transpose()
    
    if DEPURATION:
        print("File name: " + str(file))
        print(text)
        print()
    
    return text

# Generate N size sets of words, in our case game slices
def GenerateNgrams(text, N):
    
    
    # Store words (slices)
    words = []
    # Store ngrams sets
    ngrams = {}
    
    # Iterate over the text rows
    for i in range(text.shape[0]): 

        word = ""
        
        # Create a word (set of numbers separated by ','), we need to do that 
        # because each line (array) of the matrix is a word not a sentence.
        # Previously we treated each array of the matrix as a sentence and the results were horrible: 
        # e.g. "1-5.csv" (will be adjunt)
        for j in text[i]:
            word = word + str(j) + ","
            
        # Remove the last ',' because it is not needed   
        word = word[:-1]
        # Save all the words
        words.append(word)
    

    # Create all the ngrams sets
    for i in range(len(words) - N):
        sequence = ' '.join(words[i: i + N])

        if  sequence not in ngrams.keys():
            ngrams[sequence] = []
            
        ngrams[sequence].append(words[i + N])
        
    if DEPURATION: 
        print("Words: " + str(words) + "\n")
        print("Ngrmas: " + str(ngrams) + "\n")
        print()

    return ngrams, words


# In case we want to train our ngrams with more than one file
# we must create an all combined words list without repetitions
# and a ngrams with all posibilities of both files
def Multiple(n = N):
    ngramsjoin = {}
    wordsjoin = []

    for f in FILE:
        auxngrams, auxwords = GenerateNgrams(ReadFile(f), n)
        if not ngramsjoin:
            ngramsjoin = auxngrams
        else:
            new = {}
            for key, value in auxngrams.items():
                if key not in ngramsjoin:
                    new[key] = value
                else:
                    new[key] = value + ngramsjoin[key]
        
            for key, value in ngramsjoin.items():
                if key not in auxngrams:
                    new[key] = value
                else:
                    new[key] = value + auxngrams[key]
            ngramsjoin = new

        wordsjoin += auxwords

    return ngramsjoin, wordsjoin
        


# if DEPURATION:
#     print("FILES: " + str(NFILES))
#     for f in FILE:
#         print("   FILE: " + str(f))
#     print()
#     print("NGRAMS: " + str(N))
#     print()


# To save the different results of each N-gram in case that
# interpolation be true
ngramList = []
wordsList = []

if NFILES == 1:

    #Read the input file
    text = ReadFile(FILE[0])


    if(INTERPOLATION == 1):
        # Retrieve the ngram sets and the possible words
        for i in range(N):
            ngrams, words = GenerateNgrams(text, i + 1)
            ngramList.append(ngrams)
            wordsList.append(words)

        if DEPURATION:
            print("Generating interpolated " + str(N) + "grams: " + str(ngramList))
            print()
    else:
        ngrams, words = GenerateNgrams(text, N)
        ngramList.append(ngrams)
        wordsList.append(words)
        if DEPURATION:
            print("Generating " + str(N) + "grams: " + str(ngramList))
            print()



else:
    if DEPURATION:
        print("Multiple input files")

    if(INTERPOLATION == 1):
        for i in range(N):
            ngrams, words = Multiple(i + 1)
            ngramList.append(ngrams)
            wordsList.append(words)
        if DEPURATION:
            print("Generating interpolated " + str(N) + "grams: " + str(ngramList))
            print()

    else:
        ngrams, words = Multiple()
        ngramList.append(ngrams)
        wordsList.append(words)
        if DEPURATION:
            print("Generating " + str(N) + "grams: " + str(ngramList))
            print()

try:
    os.mkdir('./NgramsTraining/')
except FileExistsError:
    pass

trainFileName = time.strftime("%Y%m%d_%H%M%S")

with open('./NgramsTraining/' + trainFileName + '_Training_Ngrams.pkl', "wb") as f:
    pickle.dump(N, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(ngramList, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(wordsList, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(INTERPOLATION, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(LAMBDAPERCENT, f, pickle.HIGHEST_PROTOCOL)


