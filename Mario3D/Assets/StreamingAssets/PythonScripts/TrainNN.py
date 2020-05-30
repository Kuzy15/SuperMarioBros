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

#EXAMPLE COMMAND: python TrainNN.py 1 1-1.csv 15 20 10000 256 1024 10 2 GRU LSTM 0.5
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

NFILES = int(sys.argv[1])
FILE = []
for f in range(NFILES):
    FILE.append(str(sys.argv[f + 2]))

# The maximum length sentence we want for a single input in characters
SEQLENGTH = int(sys.argv[NFILES + 2])

# Batch size
BATCHSIZE = int(sys.argv[NFILES + 3])

# Buffer size to shuffle the dataset
# (TF data is designed to work with possibly infinite sequences,
# so it doesn't attempt to shuffle the entire sequence in memory. Instead,
# it maintains a buffer in which it shuffles elements).
BUFFERSIZE = int(sys.argv[NFILES + 4])

# The embedding dimension
EMBEDDINGDIM = int(sys.argv[NFILES + 5])

# Number of NN units
NNUNITS = int(sys.argv[NFILES + 6])

#Number of times to train
EPOCHS = int(sys.argv[NFILES + 7])

#Number of functional hidden layers
NHIDDENLAYERS = int(sys.argv[NFILES + 8])

HIDDENLAYERS = []
for l in range(NHIDDENLAYERS):
    HIDDENLAYERS.append(str(sys.argv[NFILES + l + 9]))


#Give back more or less random results
TEMPERATURE = float(sys.argv[NFILES + NHIDDENLAYERS + 9])


DEPURATION = False
if(len(sys.argv) > NFILES + NHIDDENLAYERS + 10):
    if str(sys.argv[NFILES + NHIDDENLAYERS + 10]) == "-d" or str(sys.argv[NFILES + NHIDDENLAYERS + 10]) == "--debug":
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

    return len(text) // (seqLength + 1)


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


if DEPURATION: 
    print("FILES: " + str(NFILES))
    for f in FILE:
        print("   FILE: " + str(f))
    print()
    print("SEQUENCE LENGHT: " + str(SEQLENGTH))
    print()
    print("BATCH SIZE: " + str(BATCHSIZE))
    print()
    print("BUFFER SIZE: " + str(BUFFERSIZE))
    print()
    print("EMBEDDING DIM: " + str(EMBEDDINGDIM))
    print()
    print("NN UNITS: " + str(NNUNITS))
    print()
    print("EPOCHS: " + str(EPOCHS))
    print()
    print("LAYERS: " + str(NHIDDENLAYERS))
    for l in HIDDENLAYERS:
        print("   LAYER: " + str(l))
    print()
    print("TEMPERATURE: " + str(TEMPERATURE))
    print()
    
print("Generating neural network:")
print()
#Auxiliar variables to store the first sequence to generate text
listDatasets = []
vocab = []
textstr = []

for f in FILE:
    text = ReadFile(f) 
    v, tstr = GetVocabulary(text)
    vocab += (v)
    textstr.append(tstr) 

vocab = set(vocab)


char2idx, idx2char = VectorizeText(vocab)
#print(idx2char.size)

textint = []
for t in textstr:
    textint.append(np.array([char2idx[s] for s in t]))


firstSeq = False
firstSequenceToUse = ""
datasets = []
for t in textint:
    print(t)
    print()
    firstSequence, charDataset = CreateTrainingSamples(t, idx2char)
    if not firstSeq:
        firstSequenceToUse = firstSequence
        firstSeq = True

    sequencesCreated = CreateSequences(SEQLENGTH, charDataset)

    dataset = sequencesCreated.map(SplitInputTarget)

    dataset = dataset.shuffle(BUFFERSIZE, False).batch(GetExamplesPerEpoch(t, SEQLENGTH), drop_remainder=True)
    datasets.append(dataset)





# Length of the vocabulary in chars
vocabSize = len(vocab)
model = BuildModel(vocabSize = vocabSize, embeddingDim=EMBEDDINGDIM, nnUnits=NNUNITS, batchSize=BATCHSIZE)
model.summary()
model.compile(optimizer='adam', loss=Loss)

# Directory where the checkpoints will be saved
checkpointDir = './NNTraining/cp.ckpt'
# Name of the checkpoint files
# checkpointPrefix = os.path.join(checkpointDir, "ckpt_{epoch}")

checkpointCallback = tf.keras.callbacks.ModelCheckpoint(filepath=checkpointDir, save_weights_only=True)

for d in datasets:
    model.fit(d, epochs=EPOCHS, callbacks=[checkpointCallback])


try:
    os.mkdir('./NNTraining/')
except FileExistsError:
    pass

trainFileName = time.strftime("%Y%m%d_%H%M%S")

with open('./NNTraining/' + trainFileName + '_Training_NN.pkl', "wb") as f:
    pickle.dump(char2idx, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(idx2char, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(vocabSize, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(EMBEDDINGDIM, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(NNUNITS, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(firstSequenceToUse, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(NHIDDENLAYERS, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(HIDDENLAYERS, f, pickle.HIGHEST_PROTOCOL)
    pickle.dump(TEMPERATURE, f, pickle.HIGHEST_PROTOCOL)