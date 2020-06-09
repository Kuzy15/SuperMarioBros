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


import NN

import time
import pickle

def main():

    NN.ReadArgsForTrainning()

    print("AAAAAA")
    print(NN.SEQLENGTH)

    if NN.DEPURATION:
        print("FILES: " + str(NN.NFILES))
        for f in NN.FILE:
            print("   FILE: " + str(f))
        print()
        print("SEQUENCE LENGHT: " + str(NN.SEQLENGTH))
        print()
        print("BATCH SIZE: " + str(NN.BATCHSIZE))
        print()
        print("BUFFER SIZE: " + str(NN.BUFFERSIZE))
        print()
        print("EMBEDDING DIM: " + str(NN.EMBEDDINGDIM))
        print()
        print("NN UNITS: " + str(NN.NNUNITS))
        print()
        print("EPOCHS: " + str(NN.EPOCHS))
        print()
        print("LAYERS: " + str(NN.NHIDDENLAYERS))
        for l in NN.HIDDENLAYERS:
            print("   LAYER: " + str(l))
        print()
        print("TEMPERATURE: " + str(NN.TEMPERATURE))
        print()

    print("Generating neural network:")
    print()
    #Auxiliar variables to store the first sequence to generate text
    listDatasets = []
    vocab = []
    textstr = []

    for f in NN.FILE:
        text = NN.ReadFile(f)
        v, tstr = NN.GetVocabulary(text)
        vocab += (v)
        textstr.append(tstr)

    vocab = set(vocab)


    char2idx, idx2char = NN.VectorizeText(vocab)
    #print(idx2char.size)

    textint = []
    for t in textstr:
        textint.append(NN.np.array([char2idx[s] for s in t]))


    firstSeq = False
    firstSequenceToUse = ""
    datasets = []
    for t in textint:
        print(t)
        print()
        firstSequence, charDataset = NN.CreateTrainingSamples(t, idx2char)
        if not firstSeq:
            firstSequenceToUse = firstSequence
            firstSeq = True

        sequencesCreated = NN.CreateSequences(NN.SEQLENGTH, charDataset)

        dataset = sequencesCreated.map(NN.SplitInputTarget)

        dataset = dataset.shuffle(NN.BUFFERSIZE, False).batch(NN.GetExamplesPerEpoch(t, NN.SEQLENGTH), drop_remainder=True)
        datasets.append(dataset)


    # Length of the vocabulary in chars
    vocabSize = len(vocab)
    model = NN.BuildModel(vocabSize = vocabSize, embeddingDim=NN.EMBEDDINGDIM, nnUnits=NN.NNUNITS, batchSize=NN.BATCHSIZE)
    model.summary()
    model.compile(optimizer='adam', loss=NN.Loss)

    # Directory where the checkpoints will be saved
    checkpointDir = './NNTraining/cp.ckpt'
    # Name of the checkpoint files
    # checkpointPrefix = os.path.join(checkpointDir, "ckpt_{epoch}")

    checkpointCallback = NN.tf.keras.callbacks.ModelCheckpoint(filepath=checkpointDir, save_weights_only=True)

    for d in datasets:
        model.fit(d, epochs=NN.EPOCHS, callbacks=[checkpointCallback])


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



if __name__ == "__main__":
  main()
