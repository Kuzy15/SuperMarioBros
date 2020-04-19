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
#The generated output file contains all "-1" numbers like decimals (1.0) for that reason
#we must open the outputfile in a texteditor like sublime and replace all ".0" characters
#by nothing.
# -----------------------------------------------------------

from nltk.util import bigrams
from nltk.util import ngrams
from nltk import word_tokenize
import numpy as np

import sys
import argparse
import random
import os
import csv

#parser = argparse.ArgumentParser()
#parser.add_argument("-v", "--verbose", help="Mostrar información de depuración", action="store_true")
#parser.add_argument("-v", "--verbose", help="Mostrar información de depuración", action="store_true")
#parser.add_argument("-v", "--verbose", help="Mostrar información de depuración", action="store_true")
#parser.add_argument("-v", "--verbose", help="Mostrar información de depuración", action="store_true")
#args = parser.parse_args()
 

# Aquí procesamos lo que se tiene que hacer con cada argumento
#if args.verbose:
#    print "depuración activada!!!"

NFILES = int(sys.argv[1])
FILE = []
for f in range(NFILES):
    FILE.append(str(sys.argv[f + 2]))

N = int(sys.argv[NFILES + 2])
WIDTH = int(sys.argv[NFILES + 3])
OUTPUT = str(sys.argv[NFILES + 4])



def Multiple():
    ngramsjoin = {}
    wordsjoin = []

#alternative 
#    for f in FILE:
#        auxngrams, auxwords = GenerateNgrams(ReadFile(f), N)
#        ngramsjoin = {**ngramsjoin, **auxngrams}
#        for key, value in ngramsjoin.items():
#            if key in ngramsjoin and key in auxngrams:
#                for k in ngramsjoin[key]:
#                    ngramsjoin[key].append(k)
################

    for f in FILE:
        auxngrams, auxwords = GenerateNgrams(ReadFile(f), N)
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

################

        wordsjoin += auxwords

    return ngramsjoin, wordsjoin



# Read a file and is delimited by ','
def ReadFile(file):
    
    text = np.genfromtxt(file, delimiter=',',dtype=None)
    text = text.transpose()
    
    #print(text)
    #print()
    
    return text

# Generate N size sets of words, in our case game slices
def GenerateNgrams(text, N):
    
    #print("Text: ")
    #print(text)
    
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
        
    #print("Words: " + str(words))

    # Create all the ngrams sets
    for i in range(len(words) - N):
        sequence = ' '.join(words[i: i + N])

        if  sequence not in ngrams.keys():
            ngrams[sequence] = []
            
        ngrams[sequence].append(words[i + N])
        
    return ngrams, words
        
        
# Generate the new text (map)
# "text": the generated matrix by the input file
# "N": the size of grams
# "width": the length of the text (map)
def GenerateText(ngrams, words, N, width):
  
    # The first N words of the text i.e. ngram
    currentSequence = ' '.join(words[0:N])
    # This variable is used for continuing the generation of the text when we aren't able to generate the next word
    firstSequence = currentSequence
    # Append the first sequence to the output text
    output = currentSequence
    
    # Iterate to create the new words (vertical slices) that are part of the map
    i = 0
    while(i < width):
        if currentSequence not in ngrams.keys():
            currentSequence = firstSequence
            possibleWords = ngrams[currentSequence]
            nextWord = possibleWords[random.randrange(len(possibleWords))]
            output += ' ' + currentSequence + ' ' + nextWord
            #print("output reset:" + str(output))
            i += N
            
        else:
            possibleWords = ngrams[currentSequence]
            #print("Possible words: " + str(possibleWords))
            nextWord = possibleWords[random.randrange(len(possibleWords))]
            #print("Next words: " + str(nextWord))
            output += ' ' + nextWord
            
        wordsSequence = output
        auxSequence = list(wordsSequence.split(' '))
        currentSequence = auxSequence[len(auxSequence) - N:len(auxSequence)]
        currentSequence = ' '.join(currentSequence)
        #print("currentSequencent seq: " + str(currentSequence))
        i += 1
 
    output = list(output.split(' '))
    
    return output

# Save the generated map into a file (.csv)
def SaveFile(fileName, text, width, height):   
    
    matrix = np.zeros((width, height))
    matrix = matrix.astype(int)

    i = 0
    for i in range(len(text)):
        line = text[i].split(',')
        matrix[i] = line

    #print(matrix.T)

    matrix = matrix.T
    
    csvoutput = open(fileName, 'w', newline='')
    result = csv.writer(csvoutput)
    result.writerows(matrix)
    del result
    csvoutput.close()



if NFILES == 1:

    #Read the input file
    text = ReadFile(FILE[0])

    # Retrieve the ngram sets and the possible words
    ngrams, words = GenerateNgrams(text, N)

else:
    print("multiple")
    ngrams, words = Multiple()

#The sum of N size gram and the length must be equal to the output matrix
#(e.g N = 3 and total width of map = 224, length must be 221)
output = GenerateText(ngrams, words, N, WIDTH - N)
#print(len(output))

SaveFile(OUTPUT, output, WIDTH, 15)


