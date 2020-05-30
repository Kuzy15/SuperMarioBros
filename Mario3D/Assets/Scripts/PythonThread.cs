﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;

/// <summary>
/// Class that allows us to communicate with NGRAMS.py and NeuralNetworks.py thorugh python commands on cmd and C#
/// </summary>
public class PythonThread : MonoBehaviour
{
    //PAth of where to find the resources
    private static string _path;

    // Start is called before the first frame update
    private void Start()
    {
        _path = Application.streamingAssetsPath;
    }

    /// <summary>
    /// This method executes a command on a thread
    /// </summary>
    public static void ExecuteCommand(string _command)
    {
        var thread = new Thread(delegate () { Command(_command); });
        thread.Start();
        thread.Join();
    }

    /// <summary>
    /// This method manages a command to send via cmd
    /// </summary>
    static void Command(string _command)
    {
        string command = _command;
        var processInfo = new ProcessStartInfo("cmd.exe", command);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        //Debug.Log("Ngrams: " + InputFieldManager.GM.GetNGramsInput() + "   NFiles: " + InputFieldManager.GM.GetNGramsInput() + "   Files: " + InputFieldManager.GM.GetNGramsInput());
        processInfo.WorkingDirectory = _path + "/PythonScripts";

        var process = Process.Start(processInfo);
        process.WaitForExit();
        process.Close();
    }
}   
