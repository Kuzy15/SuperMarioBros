using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;


public class PythonThread : MonoBehaviour
{
    // Start is called before the first frame update
    private static string _path;

    private void Start()
    {
        _path = Application.dataPath;
    }

    public static void ExecuteCommand()
    {
        var thread = new Thread(delegate () { Command(); });
        thread.Start();
        thread.Join();
    }

    static void Command()
    {
        string command = InputFieldManager.GM.SendCommand();
        var processInfo = new ProcessStartInfo("cmd.exe", command);
        processInfo.CreateNoWindow = false;
        processInfo.UseShellExecute = true;
        //Debug.Log("Ngrams: " + InputFieldManager.GM.GetNGramsInput() + "   NFiles: " + InputFieldManager.GM.GetNGramsInput() + "   Files: " + InputFieldManager.GM.GetNGramsInput());
        processInfo.WorkingDirectory = _path + "/Resources/Maps";

        var process = Process.Start(processInfo);
        process.WaitForExit();
        process.Close();
    }
}   
