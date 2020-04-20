using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;


public class PythonThread : MonoBehaviour
{
    // Start is called before the first frame update
    static string path;
    private void Start()
    {
        path = Application.dataPath;
    }

    public static void ExecuteCommand()
    {
        var thread = new Thread(delegate () { Command(); });
        thread.Start();
        thread.Join();
    }

    static void Command()
    {
        int nFiles = InputFieldManager.GM.GetFilesSelectedLength();
        string concat = "";
        for(int i = 0; i < nFiles; i++)
        {
            string file = InputFieldManager.GM.GetFilesToConcatInput()[i];
            string sufix = ".csv";
            string concatS = file + sufix;
            concat = concat + " " + concatS;
        }
        string debug = "";
        bool debugMode = InputFieldManager.GM.GetCheckBoxActive();
        if (debugMode)
        {
            debug = " -d";
        }
        string command = "/C python NGrams.py " + nFiles.ToString() + concat + " " + InputFieldManager.GM.GetNGramsInput() + " " + InputFieldManager.GM.GetLengthInput() + " " + "1-14.csv" + debug;
        var processInfo = new ProcessStartInfo("cmd.exe", command);
        processInfo.CreateNoWindow = false;
        processInfo.UseShellExecute = true;
        //Debug.Log("Ngrams: " + InputFieldManager.GM.GetNGramsInput() + "   NFiles: " + InputFieldManager.GM.GetNGramsInput() + "   Files: " + InputFieldManager.GM.GetNGramsInput());
        processInfo.WorkingDirectory = path + "/Maps";

        var process = Process.Start(processInfo);
        process.WaitForExit();
        process.Close();
    }
}   
