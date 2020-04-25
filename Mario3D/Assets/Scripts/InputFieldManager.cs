using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputFieldManager : MonoBehaviour
{

    public static InputFieldManager GM;
    public GameObject nGramsField;
    public GameObject lengthField;
    public GameObject nFilesField;
    public GameObject filesToConcatenateField;
    public GameObject filesScrollView;
    public PythonThread testObject;
    public GameObject filesHolder;
    public GameObject continueButton;
    public GameObject checkBox;
    public List<string> arrFiles = new List<string>();

    private string _nGramsInput;
    private string _lengthInput;
    private string _nFilesInput;
    private string _filesToConcatenateInput;
    private bool _checkBoxActive;
    private string _path;
    private string[] _files;
    private string[] _fileNames;
    private int n = 0;

    void Awake()
    {
        if (GM != null)
            GameObject.Destroy(GM);
        else
            GM = this;

        //DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        lengthField.SetActive(false);
        nFilesField.SetActive(false);
        filesToConcatenateField.SetActive(false);
        filesScrollView.SetActive(false);
        continueButton.SetActive(false);
        _checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
        checkBox.SetActive(false);
        n = 0;
        GetFileNames();
    }


    public void SetNGramsInput()
    {
        _nGramsInput = nGramsField.GetComponentInChildren<InputField>().text;
        nGramsField.SetActive(false);
        lengthField.SetActive(true);
    }

    public string GetNGramsInput()
    {
        return _nGramsInput;
    }

    public void SetLengthInput()
    {
        _lengthInput = lengthField.GetComponentInChildren<InputField>().text;
        lengthField.SetActive(false);
        SetNFilesInput();
    }

    public string GetLengthInput()
    {
        return _lengthInput;
    }

    public void SetNFilesInput()
    {
        filesScrollView.SetActive(true);
        PopulateGrid.GM.Populate(GetFilesLength(), GetFilesNames());
        continueButton.SetActive(true);
        checkBox.SetActive(true);
    }

    public string GetNFilesInput()
    {
        return _nFilesInput;
    }

    public List<string> GetFilesToConcatInput()
    {
        return arrFiles;
    }

    public void GetFileNames()
    {
        _path = Application.dataPath + "/Resources/Maps";
        _files = System.IO.Directory.GetFiles(_path, "*.csv");
        _fileNames = new string[_files.Length];
        for (int i = 0; i < _files.Length; i++)
        {
            string file = System.IO.Path.GetFileNameWithoutExtension(_files[i]);
            _fileNames[i] = file;
        }
    }

    public string[] GetFilesNames()
    {
        return _fileNames;
    }

    public int GetFilesLength()
    {
        return _files.Length;
    }

    public void AddFile(string item)
    {
        filesScrollView.transform.GetChild(0).GetComponent<Text>().text += " " + item;
        arrFiles.Add(item);
    }

    public void RemoveFile(string item)
    {
        arrFiles.RemoveAt(arrFiles.IndexOf(item));
        ResetText();
    }

    private void ResetText()
    {
        string textFiles;
        textFiles = filesScrollView.transform.GetChild(0).GetComponent<Text>().text = "Selected Files: ";
        for (int i = 0; i<arrFiles.Count; i++)
        {
            textFiles += " " + arrFiles[i];
        }
        filesScrollView.transform.GetChild(0).GetComponent<Text>().text = textFiles;
    }

    public int GetFilesSelectedLength()
    {
        return arrFiles.Count;
    }

    public bool GetCheckBoxActive()
    {
        return _checkBoxActive;
    }

    public void OnClickContinue()
    {
        if (arrFiles.Count > 0)
        {
            PythonThread.ExecuteCommand();
            MapReader.GM.InitMap(14);
            SceneManager.LoadScene(1);
        }
    }

    public void OnCheckBoxValueChanged()
    {
        _checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
    }
}
