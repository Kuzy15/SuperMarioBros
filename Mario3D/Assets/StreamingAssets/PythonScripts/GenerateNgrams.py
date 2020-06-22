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

# EXAMPLE COMMANDS:
#
# python GenerateNGrams.py training_model.pkl 250 ngramMap.csv (-d/--debug)
#


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
import math


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

            i += 1
            
        wordsSequence = output
        auxSequence = list(wordsSequence.split(' '))
        currentSequence = auxSequence[len(auxSequence) - N:len(auxSequence)]
        currentSequence = ' '.join(currentSequence)

        if DEPURATION:
            print("Next step sequence: " + str(currentSequence))
            print()

 
    output = list(output.split(' '))
    
    return output



# Generate the new text (map)
# "ngrams": ensemble with all words and posible next words
# "words": the generated matrix by the input file
# "N": the size of grams
# "width": the length of the text (map)
# "lambdapercent": percentages of lambda values for n-grams interpolation
def GenerateInterpolatedText(ngrams, words, N, width, lambdapercent):
  
    # The first N words of the text i.e. ngram
    # In case that interpolation is true we take the greatest Ngrams
    currentSequence = ' '.join(words[N-1][0:N])
    # This variable is used for continuing the generation of the text when we aren't able to generate the next word
    firstSequence = currentSequence
    if DEPURATION:
        print("First sequence: " + str(firstSequence))

    # Append the first sequence to the output text
    output = currentSequence

    # In case that interpolation is true we take the greatest Ngrams
    chosenNgram = ngrams[N-1]

    # Unkown slice to complete probabilities
    unknown = "-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1"

    # Iterate to create the new words (vertical slices) that are part of the map
    i = 0
    while(i < width):
        print("currentSequence")
        print(currentSequence)
        if currentSequence not in chosenNgram.keys():
            print("reset")
            currentSequence = firstSequence
            chosenNgram = ngrams[N-1]
            possibleWords = chosenNgram[currentSequence]
            nextWord = possibleWords[random.randrange(len(possibleWords))]
            output += ' ' + currentSequence + ' ' + nextWord

            if DEPURATION:
                print("Output reset:" + str(output))
                print()

            i += N
            
        else:
            print("start")
            print()
            interN = N
            interIndx = 0
            probabilities = []
            auxdict = {}

            # Get the probabilities of each n-gram key
            while(interN > 0):
                print("ngrams" + str(interN - 1))
                chosenNgram = ngrams[interN - 1]
                auxseq = currentSequence.split(" ")
                auxseq = ' '.join(auxseq[interIndx:])
                print("auxseq")
                print(auxseq)
        
                # Get the possible next keys of major gram, count it and then get the probability
                for s in chosenNgram[auxseq]:
                    auxdict[str(s)]= str(chosenNgram[auxseq].count(s) / len(chosenNgram[auxseq]))

                probabilities.append(auxdict)

                print("probabilities of " + str(interN) + "gram")
                print(probabilities)
                auxdict = {}
                auxseq = ""
                interN -= 1
                interIndx += 1

            print("end - probabilities")
       
            print("second round")

            chosenNgram = ngrams[N-1]
            probKeys = probabilities[-1].keys()

            probValues = []
            percent = []
            aux = 0

            # Get the percentage of each possible next key
            for k in probKeys:
                aux = 0
                probValues = []
                for p in probabilities:
                    print("probabilities")
                    print(p)
                    if k in p.keys():
                        probValues.append(float(p[k]) * float(lambdapercent[aux]))
                        print(probValues)
                    aux += 1
                percent.append(sum(probValues))
                print("key")
                print(k)
                print("percent")
                print(percent)


            listProbKeys = list(probKeys)

            # Depends on the decimals sometimes the result of the sum could be 1.00...1 or 0.999.. for this reason we round it
            # In case that it is less than 1 we add with the rest the unknown sequence
            if(sum(percent) < 1):
                rest = 1 - sum(percent)
                percent.append(rest)
                listProbKeys.append(unknown)

            # In case that it is greater than one we distribute the leftover porcentage
            if(sum(percent) > 1):
                rest = sum(percent) - 1
                rest = rest / len(percent)
                cont = 0
                while(rest != 0):
                    if percent[cont] > rest:
                        percent[cont] -= rest
                        rest = 0
                    cont += 1
                print(percent)



            #Pick the a ngram based on each percentage
            nextWord = ""
            # if(len(percent) > 1):
            nextWord = np.random.choice(listProbKeys, 1, p=percent)[0]
            # else:
            #     nextWord = list(probKeys)[0]
            
            if DEPURATION:
                print("Possible words: " + str(possibleWords))
                print("Next word: " + str(nextWord))
                print()

            output += ' ' + nextWord
            i += 1
            
        wordsSequence = output
        auxSequence = list(wordsSequence.split(' '))
        currentSequence = auxSequence[len(auxSequence) - N:len(auxSequence)]
        currentSequence = ' '.join(currentSequence)

        if DEPURATION:
            print("Next step sequence: " + str(currentSequence))
            print()
        print("vuelta" + str(i))

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
    ngramList = pickle.load(f)
    wordsList = pickle.load(f)
    INTERPOLATION = pickle.load(f)
    if(INTERPOLATION == 1):
        LAMBDAPERCENT = pickle.load(f)


#The sum of N size gram and the length must be equal to the output matrix
#(e.g N = 3 and total width of map = 224, length must be 221)
if(INTERPOLATION == 0):
    output = GenerateText(ngramList[0], wordsList[0], N, WIDTH - N)
else:
    output = GenerateInterpolatedText(ngramList, wordsList, N, WIDTH - N, LAMBDAPERCENT)

SaveFile(OUTPUT, output, len(output), 60)

if DEPURATION:
    sys.stdout.close()