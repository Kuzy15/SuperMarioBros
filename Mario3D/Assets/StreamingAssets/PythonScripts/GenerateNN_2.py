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

    with open('./NNTraining/' + TRAINFILE, 'rb') as f:
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

    genText = NN.GenerateText(model, firstSequenceToUse, NN.WIDTH)

    NN.SaveFile(NN.OUTPUT, genText)


if __name__ == "__main__":
  main()
