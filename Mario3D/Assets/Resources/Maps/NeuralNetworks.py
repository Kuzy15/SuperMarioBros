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

#EXAMPLE COMMAND: python NeuralNetworks.py 2 1-1.csv 1-2.csv 15 10000 256 1024 50 2 GRU LSTM 0.5 250 LSTM_UNITY_1.csv
#in case you want a default network you must put "1 default" in NHIDDENLAYERS and NHIDDENLAYERS respectively in the
#command line


import tensorflow as tf

import numpy as np

import os
import time
import random
import csv
import sys

try:
    from StringIO import StringIO ## for Python 2
except ImportError:
    from io import StringIO ## for Python 3


NFILES = int(sys.argv[1])
FILE = []
for f in range(NFILES):
    FILE.append(str(sys.argv[f + 2]))

print(FILE)

# The maximum length sentence we want for a single input in characters
SEQLENGTH = int(sys.argv[NFILES + 2])
print(SEQLENGTH)

# Batch size
BATCHSIZE = int(sys.argv[NFILES + 3])
print(BATCHSIZE)
# Buffer size to shuffle the dataset
# (TF data is designed to work with possibly infinite sequences,
# so it doesn't attempt to shuffle the entire sequence in memory. Instead,
# it maintains a buffer in which it shuffles elements).
BUFFERSIZE = int(sys.argv[NFILES + 4])
print(BUFFERSIZE)
# The embedding dimension
EMBEDDINGDIM = int(sys.argv[NFILES + 5])
print(EMBEDDINGDIM)
# Number of NN units
NNUNITS = int(sys.argv[NFILES + 6])
print(NNUNITS)
#Number of times to train
EPOCHS = int(sys.argv[NFILES + 7])
print(EPOCHS)
#Number of functional hidden layers
NHIDDENLAYERS = int(sys.argv[NFILES + 8])
print(NHIDDENLAYERS)

HIDDENLAYERS = []
for l in range(NHIDDENLAYERS):
    HIDDENLAYERS.append(str(sys.argv[NFILES + l + 9]))
print(HIDDENLAYERS)
print()

#Give back more or less random results
TEMPERATURE = float(sys.argv[NFILES + NHIDDENLAYERS + 9])
print(TEMPERATURE)
#The with of the level (length)
WIDTH = int(sys.argv[NFILES + NHIDDENLAYERS + 10])
print(WIDTH)
#Output name
OUTPUT = str(sys.argv[NFILES + NHIDDENLAYERS + 11])
print(OUTPUT)


# Read a file that is delimited by ',' and return a matrix of the level
def ReadFile(file):
    
    text = np.genfromtxt(file, delimiter=',',dtype=None)
    text = text.transpose()
    
    #print(text)
    #print()
    
    return text


#Vectorize the text and return the text as character, the text as integers
#and the vocabulary of the text
#"textstr": list of slices of the map
#"char2idx": to translete from char to int (id)
#"idx2char": to translate from int (id) to char
#"textint": list of unique ids of the text after parsing with "char2idx"
#"vocab": list with the vocabulary of the text
def VectorizeText(text, index):

    #Convert the matrox to a strtext
    textstr=[]
    for t in text:
        textstr.append(' '.join([str(elem) for elem in t]))

    # Get the unique characters
    vocab = sorted(set(textstr))
    #print (str(len(vocab)) + ' unique characters')
    #print((vocab))
    
    # Creating a mapping from unique characters to indices
    char2idx = {u:i + index for i, u in enumerate(vocab)}
    idx2char = np.array(vocab)
    
    textint = np.array([char2idx[c] for c in textstr])
    
    return textstr, char2idx, idx2char, textint, vocab


#Number of examples to evaluate in each epoch
def GetExamplesPerEpoch(text, seqLength):
    return len(text) // (seqLength + 1)


# Create training examples / targets
def CreateTrainingSamples(textint, idx2char):

    charDataset = tf.data.Dataset.from_tensor_slices(textint)

    for i in charDataset.take(1):
        #print(i)
        firstSeq = idx2char[i.numpy()]
        #print(charDataset.take(1))

    #charDataset is the same as textint buit in tf format
    listDataset = list(charDataset.as_numpy_iterator())
    firstSeq = idx2char[listDataset[0]]
    
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
    # model = tf.keras.Sequential([
    #     tf.keras.layers.Embedding(vocabSize, embeddingDim, batch_input_shape=[batchSize, None]),
        
    #     ##tf.keras.layers.SimpleRNN(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
    #     #tf.keras.layers.SimpleRNN(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        
    #     #tf.keras.layers.GRU(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
    #     #tf.keras.layers.GRU(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        
    #     #tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
    #     #tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        
    #     tf.keras.layers.Dense(vocabSize)])

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

    return model


#Loss function for neural network model
def Loss(labels, logits):
    return tf.keras.losses.sparse_categorical_crossentropy(labels, logits, from_logits=True)


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
    for i in range(numGenerate):
        predictions = model(inputEval)
      # remove the batch dimension
        predictions = tf.squeeze(predictions, 0)

      # using a categorical distribution to predict the character returned by the model
        predictions = predictions / temperature
        predictedId = tf.random.categorical(predictions, num_samples=1)[-1,0].numpy()

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





#"Empty" tensorflow dataset to store the possible multiple train data set
combinedDataset = tf.data.Dataset.range(0)

#Auxiliar variables to store the first sequence to generate text
firstFile = True
firstSequence = ""
listDatasets = []
index = 0
for f in FILE:
    text = ReadFile(f)
    print(len(text))
    textstr, char2idx, idx2char, textint, vocab = VectorizeText(text, index)
    index += len(char2idx)
    print(len(vocab))
    examplesPerEpoch = GetExamplesPerEpoch(text, SEQLENGTH)
    print(examplesPerEpoch)
    trainingSample, charDataset = CreateTrainingSamples(textint, idx2char)

    if(firstFile):
        firstSequence = trainingSample

    sequencesCreated = CreateSequences(SEQLENGTH, charDataset)

    dataset = sequencesCreated.map(SplitInputTarget)

    dataset = dataset.shuffle(BUFFERSIZE).batch(BATCHSIZE, drop_remainder=True)
    listDatasets.append(dataset)
    if firstFile:
        combinedDataset = dataset
        firstFile = False
    else:
        combinedDataset = combinedDataset.concatenate(dataset)
    print()

print()
print()
print("FIRST SEQUENCE: " + firstSequence)
print()
print()

# Length of the vocabulary in chars
vocabSize = len(vocab)
model = BuildModel(vocabSize = vocabSize, embeddingDim=EMBEDDINGDIM, nnUnits=NNUNITS, batchSize=BATCHSIZE)
model.summary()
model.compile(optimizer='adam', loss=Loss)

# Directory where the checkpoints will be saved
checkpointDir = './training_checkpoints'
# Name of the checkpoint files
checkpointPrefix = os.path.join(checkpointDir, "ckpt_{epoch}")

checkpointCallback = tf.keras.callbacks.ModelCheckpoint(filepath=checkpointPrefix, save_weights_only=True)

model.fit(combinedDataset, epochs=EPOCHS, callbacks=[checkpointCallback])




tf.train.latest_checkpoint(checkpointDir)

model = BuildModel(vocabSize, EMBEDDINGDIM, NNUNITS, batchSize=1)

model.load_weights(tf.train.latest_checkpoint(checkpointDir))

model.build(tf.TensorShape([1, None]))

model.summary()

genText = GenerateText(model, firstSequence, WIDTH)

SaveFile(OUTPUT, genText)



