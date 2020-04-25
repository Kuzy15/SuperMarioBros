using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputFieldManager : MonoBehaviour
{

    public static InputFieldManager GM;

    // Start is called before the first frame update
    public GameObject nGramsField;
    public GameObject lengthField;
    public GameObject nFilesField;
    public GameObject filesToConcatenateField;
    public GameObject filesScrollView;
    public PythonThread testObject;
    public GameObject filesHolder;
    public GameObject continueButton;
    public GameObject checkBox;

    string nGramsInput;
    string lengthInput;
    string nFilesInput;
    string filesToConcatenateInput;

    bool checkBoxActive;


    string path;
    string[] files;
    string[] fileNames;

    int n = 0;
    public System.Collections.Generic.List<string> arrFiles = new System.Collections.Generic.List<string>();
    void Awake()
    {
        if (GM != null)
            GameObject.Destroy(GM);
        else
            GM = this;

        //DontDestroyOnLoad(this);
    }

    void Start()
    {
        lengthField.SetActive(false);
        nFilesField.SetActive(false);
        filesToConcatenateField.SetActive(false);
        filesScrollView.SetActive(false);
        continueButton.SetActive(false);
        checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
        checkBox.SetActive(false);
        n = 0;
        GetFileNames();
    }

    // Update is called once per frame

    public void SetNGramsInput()
    {
        nGramsInput = nGramsField.GetComponentInChildren<InputField>().text;
        nGramsField.SetActive(false);
        lengthField.SetActive(true);
    }

    public string GetNGramsInput()
    {
        return nGramsInput;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(arrFiles.Count);
        }
    }

    public void SetLengthInput()
    {
        lengthInput = lengthField.GetComponentInChildren<InputField>().text;
        lengthField.SetActive(false);
        SetNFilesInput();
    }

    public string GetLengthInput()
    {
        return lengthInput;
    }

    public void SetNFilesInput()
    {
        filesScrollView.SetActive(true);
        PopulateGrid.GM.Populate(GetFilesLength(), GetFilesNames());
        continueButton.SetActive(true);
        checkBox.SetActive(true);
        //NewOnRight(int.Parse(nFilesInput));
    }

    public string GetNFilesInput()
    {
        return nFilesInput;
    }

    public void SetFilesToConcat()
    {
        filesToConcatenateInput = filesHolder.GetComponentsInChildren<InputField>()[n].text;
        filesToConcatenateField.SetActive(false);
        // arrFiles[n] = filesToConcatenateInput;
        n++;
        if (n >= int.Parse(nFilesInput))
        {
            //PythonThread.ExecuteCommand();
            MapReader.GM.InitMap(2);
            SceneManager.LoadScene(1);
        }
    }

    public List<string> GetFilesToConcatInput()
    {
        return arrFiles;
    }


    public void GetFileNames()
    {
        path = Application.dataPath + "/Resources/Maps";
        files = System.IO.Directory.GetFiles(path, "*.csv");
        fileNames = new string[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            string file = System.IO.Path.GetFileNameWithoutExtension(files[i]);
            fileNames[i] = file;
        }
    }

    public string[] GetFilesNames()
    {
        return fileNames;
    }

    public int GetFilesLength()
    {
        return files.Length;
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
        return checkBoxActive;
    }

    public void OnClickContinue()
    {
        if (arrFiles.Count > 0)
        {
            PythonThread.ExecuteCommand();
            MapReader.GM.InitMap(2);
            SceneManager.LoadScene(1);
        }
    }

    public void OnCheckBoxValueChanged()
    {
        checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
    }
}
