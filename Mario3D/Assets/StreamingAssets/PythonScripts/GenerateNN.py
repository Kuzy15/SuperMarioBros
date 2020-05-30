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

#EXAMPLE COMMAND: python GenerateNN.py 20200528_201621_Training_NN.pkl 250 LSTM_UNITY_1.csv
#in case you want a default network you must put "1 default" in NHIDDENLAYERS and NHIDDENLAYERS respectively in the
#command line


import tensorflow as tf

import numpy as np

import os
import time
import random
import csv
import sys
import pickle

try:
    from StringIO import StringIO ## for Python 2
except ImportError:
    from io import StringIO ## for Python 3
    

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
            sys.stdout = open(path + logFileName +"_Generated_NN.txt", "w")
        except OSError:
            if not os.path.isdir(path):
                raise


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



#Generate slices of the level from a trained model, a starting sequence and a length
def GenerateText(model, startString, length):
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
    temperature = TEMPERATURE

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


char2idx = {}
idx2char = []
vocabSize = 0
EMBEDDINGDIM = 0
NNUNITS = 0
firstSequenceToUse = ""
NHIDDENLAYERS = 0
HIDDENLAYERS = []
TEMPERATURE = 0

with open('./NNTraining/' + TRAINFILE, 'rb') as f:
    char2idx = pickle.load(f)
    idx2char = pickle.load(f)
    vocabSize = pickle.load(f)
    EMBEDDINGDIM = pickle.load(f)
    NNUNITS = pickle.load(f)
    firstSequenceToUse = pickle.load(f)
    NHIDDENLAYERS = pickle.load(f)
    HIDDENLAYERS = pickle.load(f)
    TEMPERATURE = pickle.load(f)


model = BuildModel(vocabSize, EMBEDDINGDIM, NNUNITS, batchSize=1)

# model.load_weights(tf.train.latest_checkpoint(checkpointDir))
model.load_weights('./NNTraining/cp.ckpt')

model.build(tf.TensorShape([1, None]))

model.summary()

genText = GenerateText(model, firstSequenceToUse, WIDTH)

SaveFile(OUTPUT, genText)