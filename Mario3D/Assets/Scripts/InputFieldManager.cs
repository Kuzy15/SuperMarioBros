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
    public test testObject;
    string nGramsInput;
    string lengthInput;
    string nFilesInput;
    string filesToConcatenateInput;
    public GameObject filesHolder;
    int n = 0;
    string[] arrFiles;
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
        n = 0;
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

    public void SetLengthInput()
    {
        lengthInput = lengthField.GetComponentInChildren<InputField>().text;
        lengthField.SetActive(false);
        nFilesField.SetActive(true);
    }

    public string GetLengthInput()
    {
        return lengthInput;
    }

    public void SetNFilesInput()
    {
        nFilesInput = nFilesField.GetComponentInChildren<InputField>().text;
        nFilesField.SetActive(false);
        filesToConcatenateField.SetActive(true);
        NewOnRight(int.Parse(nFilesInput));
    }

    public string GetNFilesInput()
    {
        return nFilesInput;
    }

    public void SetFilesToConcat()
    {
        filesToConcatenateInput = filesHolder.GetComponentsInChildren<InputField>()[n].text;
        filesToConcatenateField.SetActive(false);
        arrFiles[n] = filesToConcatenateInput;
        n++;
        if(n >= int.Parse(nFilesInput))
        {
            test.ExecuteCommand();
            MapReader.GM.InitMap(14);
            SceneManager.LoadScene(1);
        }
    }

    public string[] GetFilesToConcatInput()
    {
        return arrFiles;
    }

    private void NewOnRight(int n)
    {
        for(int i = 0; i < int.Parse(nFilesInput); i++)
        {
            GameObject nu = Instantiate(filesToConcatenateField.gameObject);
            nu.transform.SetParent(filesHolder.transform, true);
            float d = filesToConcatenateField.GetComponentInChildren<InputField>().GetComponent<RectTransform>().rect.height;
            nu.transform.position = new Vector3(filesToConcatenateField.transform.position.x, filesToConcatenateField.transform.position.y - (i * d * 2), filesToConcatenateField.transform.localPosition.z);
            nu.SetActive(true);
            nu.GetComponentInChildren<Text>().text = "FILE " + i + "/" + n + ":";
        }

        arrFiles = new string[int.Parse(nFilesInput)];

        /*SoundButton nusb = nu.GetComponent<SoundButton>();

        nusb.Info = s;
        nusb.theStar.enabled = false;
        nusb.topColor = daBoss.TopColorFor(s);
        nusb.bottomColor = daBoss.BottomColorFor(s);*/
    }
}
