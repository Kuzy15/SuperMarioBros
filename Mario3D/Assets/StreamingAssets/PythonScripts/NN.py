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


import tensorflow as tf

import numpy as np

import os
import random
import sys
import time
import csv

try:
    from StringIO import StringIO ## for Python 2
except ImportError:
    from io import StringIO ## for Python 3

def ReadArgsForTrainning():

    global NFILES
    NFILES = int(sys.argv[1])
    print(NFILES)
    print()
    global FILE
    FILE = []
    for f in range(NFILES):
        FILE.append(str(sys.argv[f + 2]))

    # The maximum length sentence we want for a single input in characters
    global SEQLENGTH
    SEQLENGTH = int(sys.argv[NFILES + 2])
    print(SEQLENGTH)
    print()
    # Buffer size to shuffle the dataset
    # (TF data is designed to work with possibly infinite sequences,
    # so it doesn't attempt to shuffle the entire sequence in memory. Instead,
    # it maintains a buffer in which it shuffles elements).
    global BUFFERSIZE
    BUFFERSIZE = int(sys.argv[NFILES + 3])
    print(BUFFERSIZE)
    print()
    # The embedding dimension
    global EMBEDDINGDIM
    EMBEDDINGDIM = int(sys.argv[NFILES + 4])
    print(EMBEDDINGDIM)
    print()

    # Number of NN units
    global NNUNITS
    NNUNITS = int(sys.argv[NFILES + 5])
    print(NNUNITS)
    print()
    #Number of times to train
    global EPOCHS
    EPOCHS = int(sys.argv[NFILES + 6])
    print(EPOCHS)
    print()
    #Number of functional hidden layers
    global NHIDDENLAYERS
    NHIDDENLAYERS = int(sys.argv[NFILES + 7])
    print(NHIDDENLAYERS)
    print()
    global HIDDENLAYERS
    HIDDENLAYERS = []
    for l in range(NHIDDENLAYERS):
        HIDDENLAYERS.append(str(sys.argv[NFILES + l + 8]))
        print(str(sys.argv[NFILES + l + 8]))

    print()
    #Give back more or less random results
    global TEMPERATURE
    TEMPERATURE = float(sys.argv[NFILES + NHIDDENLAYERS + 8])
    print(TEMPERATURE)
    print()

    global DEPURATION
    DEPURATION = False
    if(len(sys.argv) > NFILES + NHIDDENLAYERS + 9):
        if str(sys.argv[NFILES + NHIDDENLAYERS + 9]) == "-d" or str(sys.argv[NFILES + NHIDDENLAYERS + 9]) == "--debug":
            DEPURATION = True
            try:
                path = "../Logs/"
                if not os.path.exists(path):
                    os.makedirs(path)
                logFileName = time.strftime("%Y%m%d-%H%M%S")
                sys.stdout = open(path + logFileName +"_RNN.txt", "w")
            except OSError:
                if not os.path.isdir(path):
                    raise


def ReadArgsForGenerating():

    global TRAINFILE
    TRAINFILE = str(sys.argv[1])
    global WIDTH
    WIDTH = int(sys.argv[2])
    global OUTPUT
    OUTPUT = str(sys.argv[3])
    global DEPURATION
    DEPURATION = False

    if(len(sys.argv) > 4):
        if str(sys.argv[4]) == "-d" or str(sys.argv[4]) == "--debug":
            DEPURATION = True
            try:
                path = "../Logs/"
                if not os.path.exists(path):
                    os.makedirs(path)
                logFileName = time.strftime("%Y%m%d-%H%M%S")
                sys.stdout = open(path + logFileName +"_Generated_NN.txt", "w")
            except OSError:
                if not os.path.isdir(path):
                    raise


# Read a file that is delimited by ',' and return a matrix of the level
def ReadFile(file):

    text = np.genfromtxt(file, delimiter=',',dtype=None)
    text = text.transpose()

    if DEPURATION:
        print("File name: " + str(file))
        print(text)
        print()

    return text


#Get vocabulary from the input text
def GetVocabulary(text):
    #Convert the matrix to a strtext
    textstr=[]
    for t in text:
        textstr.append(' '.join([str(elem) for elem in t]))

    # Get the unique characters
    vocab = sorted(set(textstr))

    if DEPURATION:
	    print("Vocabulary: " + str(vocab))
	    print()

    return vocab, textstr

#Vectorize the text and return the text as character, the text as integers
#and the vocabulary of the text
#"char2idx": to translete from char to int (id)
#"idx2char": to translate from int (id) to char
def VectorizeText(vocab):

    # Creating a mapping from unique characters to indices
    char2idx = {u:i for i, u in enumerate(vocab)}
    idx2char = list(vocab)

    if DEPURATION:
    	print("char2idx: " + str(char2idx))

    return char2idx, idx2char


#Number of examples to evaluate in each epoch
def GetExamplesPerEpoch(text, seqLength):

    if DEPURATION:
        print("Examples per epoch:" + str(len(text) // (seqLength + 1)))

    examplesPerEpoch = len(text) // (seqLength + 1)


    # BATCHSIZE > examplesPerEpoch ---> sys.exit("BATCHSIZE IS GREATER THAN examplesPerEpoch")
    return examplesPerEpoch


# Create training examples / targets
def CreateTrainingSamples(textint, idx2char):

    charDataset = tf.data.Dataset.from_tensor_slices(textint)

    #charDataset is the same as textint buit in tf format
    listDataset = list(charDataset.as_numpy_iterator())

    firstSeq = idx2char[listDataset[0]]

    if DEPURATION:
    	print("Data list: " + str(listDataset))
    	print("First sequence: " + str(firstSeq))

    return firstSeq, charDataset


#Create sequences with length equal to seq_length+1 of level slices
def CreateSequences(seqLength, charDataset):
    return charDataset.batch(seqLength+1, drop_remainder= True)


#Each element of retuning value contain two arrays.
#First one is the sequence with se_length size
#Second one is the scrolled sequence
def SplitInputTarget(chunk):
    inputText = chunk[:-1]
    targetText = chunk[1:]
    return inputText, targetText


#Build a model of different types of neural networks
def BuildModel(vocabSize, embeddingDim, nnUnits, batchSize):

    model = tf.keras.Sequential()
    model.add(tf.keras.layers.Embedding(vocabSize, embeddingDim, batch_input_shape=[batchSize, None]))

    if (HIDDENLAYERS[0] == "default"):
            model.add(tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'))
            model.add(tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'))
    else:
        for hl in HIDDENLAYERS:
            if (hl == "SRNN"):
                model.add(tf.keras.layers.SimpleRNN(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'))
            elif (hl == "LSTM"):
                model.add(tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'))
            else:
                model.add(tf.keras.layers.GRU(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'))

        model.add(tf.keras.layers.Dense(vocabSize))

    if DEPURATION:
    	model.summary()

    return model


#Loss function for neural network model
def Loss(labels, logits):
    return tf.keras.losses.sparse_categorical_crossentropy(labels, logits, from_logits=True)


#Generate slices of the level from a trained model, a starting sequence and a length
def GenerateText(model, startString, length, char2idx, idx2char):
  # Evaluation step (generating text using the learned model)

  # Number of characters to generate
    numGenerate = length

  # Converting our start string to numbers (vectorizing)
    inputEval = [char2idx[startString]]
    print(inputEval)
    inputEval = tf.expand_dims(inputEval, 0)

  # Empty string to store our results
    textGenerated = []

  # Low temperatures results in more predictable text.
  # Higher temperatures results in more surprising text.
    temperature = TEMPERATURE  # TODO: CAMBIAR

  # Here batch size == 1
    model.reset_states()
    cont = 0
    for i in range(numGenerate):
        cont+=1
        print(cont)
        predictions = model(inputEval)
      # remove the batch dimension
        tfPredictions = tf.squeeze(predictions, 0)

      # using a categorical distribution to predict the character returned by the model
        predictions = tfPredictions / temperature
        predictedId = tf.random.categorical(predictions, num_samples=1)[-1,0].numpy()

        if DEPURATION:
	        print("TensorFlow predictions:")
        	print(tfPredictions)
        	print()
        	print("Predictions applying temperature:")
        	print(predictions)
        	print()
        	print("Predicted next id: " + str(predictedId))
        	print()
        	print("Next sequence: " + str(idx2char[predictedId]))
        	print()

      # We pass the predicted character as the next input to the model
      # along with the previous hidden state
        inputEval = tf.expand_dims([predictedId], 0)

        textGenerated.append(',' + idx2char[predictedId])

    # print(startString)
    # print()
    return (startString + ''.join(textGenerated))


# Save the generated map into a file (.csv)
def SaveFile(fileName, text):
    text = text.replace(',', '\n')
    text = text.replace(' ', ',')

    matrix = np.genfromtxt(StringIO(text), delimiter=',', dtype=None)
    matrix = matrix.astype(int)

    csvoutput = open(fileName, 'w', newline='')
    result = csv.writer(csvoutput)
    result.writerows(matrix.T)
    del result
    csvoutput.close()
    if DEPURATION:
        print("Generated a RNN file " + str(fileName) + " with a length of " + str(WIDTH))
