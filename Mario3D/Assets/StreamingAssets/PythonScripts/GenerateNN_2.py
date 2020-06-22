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

import os
import tensorflow as tf

import NN

import pickle

def main():

    char2idx = {}
    idx2char = []
    vocabSize = 0
    NN.EMBEDDINGDIM = 0
    NN.NNUNITS = 0
    firstSequenceToUse = ""
    NN.NHIDDENLAYERS = 0
    NN.HIDDENLAYERS = []
    NN.TEMPERATURE = 0

    NN.ReadArgsForGenerating()

    with open('./NNTraining/' + NN.TRAINFILE, 'rb') as f:
        char2idx = pickle.load(f)
        idx2char = pickle.load(f)
        vocabSize = pickle.load(f)
        NN.EMBEDDINGDIM = pickle.load(f)
        NN.NNUNITS = pickle.load(f)
        firstSequenceToUse = pickle.load(f)
        NN.NHIDDENLAYERS = pickle.load(f)
        NN.HIDDENLAYERS = pickle.load(f)
        NN.TEMPERATURE = pickle.load(f)


    model = NN.BuildModel(vocabSize, NN.EMBEDDINGDIM, NN.NNUNITS, batchSize=1)

    # model.load_weights(tf.train.latest_checkpoint(checkpointDir))
    model.load_weights('./NNTraining/cp.ckpt')

    model.build(tf.TensorShape([1, None]))

    model.summary()

    genText = NN.GenerateText(model, firstSequenceToUse, NN.WIDTH, char2idx, idx2char)

    NN.SaveFile(NN.OUTPUT, genText)


if __name__ == "__main__":
  main()
