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
#
#
#
# -----------------------------------------------------------

#EXAMPLE COMMAND: python NeuralNetworks.py 70 1-1.csv 10000 512 1024 70 0.5 LSTM_UNITY_1.csv

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


SEQLENGTH = int(sys.argv[1])
FILE = str(sys.argv[2])
BUFFERSIZE = int(sys.argv[3])
EMBEDDINGDIM = int(sys.argv[4])
NNUNITS = int(sys.argv[5])
EPOCHS = int(sys.argv[6])
TEMPERATURE = float(sys.argv[7])
OUTPUT = str(sys.argv[8])


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
def VectorizeText(text):

    #Convert the matrox to a strtext
    textstr=[]
    for t in text:
        textstr.append(' '.join([str(elem) for elem in t]))

    # Get the unique characters
    vocab = sorted(set(textstr))
    #print (str(len(vocab)) + ' unique characters')
    #print((vocab))
    
    # Creating a mapping from unique characters to indices
    char2idx = {u:i for i, u in enumerate(vocab)}
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
    model = tf.keras.Sequential([
        tf.keras.layers.Embedding(vocabSize, embeddingDim, batch_input_shape=[batchSize, None]),
        
        ##tf.keras.layers.SimpleRNN(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        #tf.keras.layers.SimpleRNN(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        
        #tf.keras.layers.GRU(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        #tf.keras.layers.GRU(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        
        tf.keras.layers.GRU(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        #tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        #tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        #tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        #tf.keras.layers.LSTM(nnUnits, return_sequences=True, stateful=True, recurrent_initializer='glorot_uniform'),
        
        tf.keras.layers.Dense(vocabSize)])
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
    #inputEval = [char2idx[s] for s in startString]
    inputEval = [char2idx[startString]]
    print(inputEval)
    inputEval = tf.expand_dims(inputEval, 0)

  # Empty string to store our results
    textGenerated = []

  # Low temperatures results in more predictable text.
  # Higher temperatures results in more surprising text.
  # Experiment to find the best setting.
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
    print(startString)
    print()
    return (startString + ''.join(textGenerated))


# Save the generated map into a file (.csv)
def SaveFile(fileName, text):
    text = text.replace(',', '\n')
    text = text.replace(' ', ',')
    print()
    #print(text)
    matrix = np.genfromtxt(StringIO(text), delimiter=',', dtype=None)
    #matrix = np.zeros((len(text), len(text[0])))
    matrix = matrix.astype(int)
    print(matrix)

    #for i in range(len(text)):
     #   line=text[i]
      #  matrix[i] = line

    csvoutput = open(fileName, 'w', newline='')
    result = csv.writer(csvoutput)
    result.writerows(matrix.T)
    del result
    csvoutput.close()








# The maximum length sentence we want for a single input in characters
seqLength = SEQLENGTH

text = ReadFile(FILE)

textstr, char2idx, idx2char, textint, vocab = VectorizeText(text)

examplesPerEpoch = GetExamplesPerEpoch(text, seqLength)

trainingSample, charDataset = CreateTrainingSamples(textint, idx2char)

sequencesCreated = CreateSequences(seqLength, charDataset)

dataset = sequencesCreated.map(SplitInputTarget)

# Batch size
batchSize = examplesPerEpoch

print()
print()
print(batchSize)
print()
print()

# Buffer size to shuffle the dataset
# (TF data is designed to work with possibly infinite sequences,
# so it doesn't attempt to shuffle the entire sequence in memory. Instead,
# it maintains a buffer in which it shuffles elements).
bufferSize = BUFFERSIZE

dataset = dataset.shuffle(bufferSize).batch(batchSize, drop_remainder=True)

# Length of the vocabulary in chars
vocabSize = len(vocab)

# The embedding dimension
embeddingDim = EMBEDDINGDIM

# Number of NN units
nnUnits = NNUNITS

model = BuildModel(vocabSize = vocabSize, embeddingDim=embeddingDim, nnUnits=nnUnits, batchSize=batchSize)

model.compile(optimizer='adam', loss=Loss)

# Directory where the checkpoints will be saved
checkpointDir = './training_checkpoints'
# Name of the checkpoint files
checkpointPrefix = os.path.join(checkpointDir, "ckpt_{epoch}")

checkpointCallback = tf.keras.callbacks.ModelCheckpoint(filepath=checkpointPrefix, save_weights_only=True)


EPOCHS = EPOCHS
history = model.fit(dataset, epochs=EPOCHS, callbacks=[checkpointCallback])

tf.train.latest_checkpoint(checkpointDir)

model = BuildModel(vocabSize, embeddingDim, nnUnits, batchSize=1)

model.load_weights(tf.train.latest_checkpoint(checkpointDir))

model.build(tf.TensorShape([1, None]))

model.summary()

genText = GenerateText(model, trainingSample, 1000)

SaveFile(OUTPUT, genText)



