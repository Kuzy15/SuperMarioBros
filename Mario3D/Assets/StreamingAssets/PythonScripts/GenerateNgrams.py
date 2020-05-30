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

#EXAMPLE COMMAND: python GenerateNGrams.py training_model.pkl 250 ngramMap.csv (-d/--debug)


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


TRAINFILE = str(sys.argv[1])
WIDTH = int(sys.argv[2])
OUTPUT = str(sys.argv[3])

DEPURATION = False
if(len(sys.argv) > 4):
    if str(sys.argv[4]) == "-d" or str(sys.argv[4]) == "--debug":
        DEPURATION = True
        try: 
            path = "../Logs/"
            if not os.path.exists(path):
                os.makedirs(path)
            logFileName = time.strftime("%Y%m%d-%H%M%S")
            sys.stdout = open(path + logFileName +"_Generated_Ngrams.txt", "w")
        except OSError:
            if not os.path.isdir(path):
                raise

   
        
# Generate the new text (map)
# "ngrams": ensemble with all words and posible next words
# "words": the generated matrix by the input file
# "N": the size of grams
# "width": the length of the text (map)
def GenerateText(ngrams, words, N, width):
  
    # The first N words of the text i.e. ngram
    currentSequence = ' '.join(words[0:N])
    # This variable is used for continuing the generation of the text when we aren't able to generate the next word
    firstSequence = currentSequence
    if DEPURATION:
        print("First sequence: " + str(firstSequence))

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

            if DEPURATION:
                print("Output reset:" + str(output))
                print()

            i += N
            
        else:
            possibleWords = ngrams[currentSequence]

            nextWord = possibleWords[random.randrange(len(possibleWords))]
            
            if DEPURATION:
                print("Possible words: " + str(possibleWords))
                print("Next word: " + str(nextWord))
                print()

            output += ' ' + nextWord
            
        wordsSequence = output
        auxSequence = list(wordsSequence.split(' '))
        currentSequence = auxSequence[len(auxSequence) - N:len(auxSequence)]
        currentSequence = ' '.join(currentSequence)

        if DEPURATION:
            print("Next step sequence: " + str(currentSequence))
            print()

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

    if DEPURATION:
        print("Generated a " + str(N) + "gram file " + str(fileName) + " with a length of " + str(width + N))




with open('./NgramsTraining/' + TRAINFILE, 'rb') as f:
    N = pickle.load(f)
    ngrams = pickle.load(f)
    words = pickle.load(f)


#The sum of N size gram and the length must be equal to the output matrix
#(e.g N = 3 and total width of map = 224, length must be 221)
output = GenerateText(ngrams, words, N, WIDTH - N)
print(len(output))

SaveFile(OUTPUT, output, WIDTH, 60)

if DEPURATION:
    sys.stdout.close()


