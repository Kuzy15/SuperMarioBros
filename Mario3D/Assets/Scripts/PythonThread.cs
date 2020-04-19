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
        string nFiles = InputFieldManager.GM.GetNFilesInput();
        string concat = "";
        for(int i = 0; i < int.Parse(nFiles); i++)
        {
            string prefix = "1-";
            string file = InputFieldManager.GM.GetFilesToConcatInput()[i];
            string sufix = ".csv";
            string concatS = prefix + file + sufix;
            concat = concat + " " + concatS;
        }
        string command = "/C python NGrams.py " + nFiles + concat + " " + InputFieldManager.GM.GetNGramsInput() + " " + InputFieldManager.GM.GetLengthInput() + " " + "1-14.csv";
        var processInfo = new ProcessStartInfo("cmd.exe", command);
        processInfo.CreateNoWindow = false;
        processInfo.UseShellExecute = false;
        //Debug.Log("Ngrams: " + InputFieldManager.GM.GetNGramsInput() + "   NFiles: " + InputFieldManager.GM.GetNGramsInput() + "   Files: " + InputFieldManager.GM.GetNGramsInput());
        processInfo.WorkingDirectory = path + "/Maps";

        var process = Process.Start(processInfo);

        process.WaitForExit();
        process.Close();
    }
}   
