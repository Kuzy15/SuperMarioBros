using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputFieldManager : MonoBehaviour
{

    public static InputFieldManager GM;
    public GameObject filesScrollView;
    public PythonThread testObject;
    public GameObject continueButton;
    public GameObject checkBox;
    public GameObject loadButton;
    public GameObject generateButton;
    public GameObject nGramsMode;
    public GameObject rnnMode;
    public List<string> arrFiles = new List<string>();
    public GameObject rnnObject;
    public GameObject rnnContinue;
    public GameObject continueScroll;
    public GameObject nGramsObject;
    public GameObject nGramsContinue;
    public GameObject layersObject;
    public Text modeText;
    public Text layersText;
    public GameObject resetLayers;


    private string _nFilesInput;
    private bool _checkBoxActive;
    private string _path;
    private string[] _files;
    private string[] _fileNames;
    private int n = 0;
    private bool _generationMode;
    private bool _mlMode;

    //RNN
    private string _seqLengthInput;
    private string _bufferSizeInput;
    private string _embedDimInput;
    private string _nnUnitsInput;
    private string _epochsInput;
    private string _temperatureInput;
    private string _fileNameRNN;
    private int _rnnSimpleLayers;
    private int _gruLayers;
    private int _lstmLayers;
    private string _batchSize;
    private string _width;
    private List<string> _layersArr = new List<string>();

    //NGRAMS
    private string _nGramsInput;
    private string _lengthInput;
    private string _fileNameNGrams;

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
        filesScrollView.SetActive(false);
        continueButton.SetActive(false);
        //continueScroll.SetActive(false);
        _checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
        checkBox.SetActive(false);
        n = 0;
        GetFileNames();
    }

    public void StartNGrams()
    {
        nGramsObject.SetActive(true);
    }

    public void StartRNN()
    {
        rnnObject.SetActive(true);
    }

    public string GetNGramsInput()
    {
        return _nGramsInput;
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
        for (int i = 0; i < arrFiles.Count; i++)
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
        if (_generationMode)
        {
            string fileToGen = "";
            if (_mlMode)
            {
                fileToGen = _fileNameNGrams;
            }
            else
            {
                fileToGen = _fileNameRNN;
            }
            if (arrFiles.Count > 0)
            {
                PythonThread.ExecuteCommand();
                MapReader.GM.InitMap(fileToGen);
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            if (arrFiles.Count == 1)
            {
                //PythonThread.ExecuteCommand();
                MapReader.GM.InitMap(arrFiles[0], false);
                SceneManager.LoadScene(1);
            }
        }
    }

    public void OnCheckBoxValueChanged()
    {
        _checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
        Debug.Log(_checkBoxActive);
    }

    public void OnClickMode(bool generation = false)
    {
        generateButton.SetActive(false);
        loadButton.SetActive(false);
        if (generation)
        {
            _generationMode = true;
            OnGeneratingMode();
        }
        else
        {
            OnLoadMode();
        }
    }

    public void OnGeneratingMode()
    {
        nGramsMode.SetActive(true);
        rnnMode.SetActive(true);
        //nGramsField.SetActive(true);
        /*SetNGramsInput();
        SetLengthInput();
        SetNFilesInput();*/
    }

    public void OnLoadMode()
    {
        SetNFilesInput();
    }

    public bool GetGenerationMode()
    {
        return _generationMode;
    }

    public void OnClickMLMode(bool nGrams)
    {
        nGramsMode.SetActive(false);
        rnnMode.SetActive(false);
        _mlMode = nGrams;
        modeText.gameObject.SetActive(true);
        if (_mlMode)
        {
            StartNGrams();
            modeText.text = "NGRAMS";
        }
        else
        {
            StartRNN();
            modeText.text = "Recurrent Neural Networks";
            //SetRNNInput();
        }
    }

    public void BackButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    //RNN

    public void SetSeqLengthInput()
    {
        _seqLengthInput = rnnObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetBufferSizeInput()
    {
        _bufferSizeInput = rnnObject.transform.GetChild(1).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetEmbedDimInput()
    {
        _embedDimInput = rnnObject.transform.GetChild(2).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetNNUnitsInput()
    {
        _nnUnitsInput = rnnObject.transform.GetChild(3).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetEpochsInput()
    {
        _epochsInput = rnnObject.transform.GetChild(4).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetTemperatureInput()
    {
        _temperatureInput = rnnObject.transform.GetChild(5).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetFileNameInput()
    {
        _fileNameRNN = rnnObject.transform.GetChild(6).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetBatchSize()
    {
        _batchSize = rnnObject.transform.GetChild(7).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetWidth()
    {
        _width = rnnObject.transform.GetChild(8).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void OnClickContinueRNN()
    {
        //ADD LAYERS INPUT
        if (CheckIfCanContinue(rnnObject))
        {
            rnnObject.SetActive(false);
            rnnContinue.SetActive(false);
            layersObject.SetActive(true);
            //SetNFilesInput();
        }
    }

    public void OnClickContinueScroll()
    {
        //ADD LAYERS INPUT
            layersObject.SetActive(false);
            SetNFilesInput();
    }

    public void OnClickRNNSimpleLayer(int quantity)
    {
        if(quantity == 1)
        {
            _layersArr.Add("SRNN");
            SetLayersText();
        }
        else
        {
            ResetLayers();
        }
        _rnnSimpleLayers += quantity;
        if (_rnnSimpleLayers < 0)
        {
            _rnnSimpleLayers = 0;
        }
        layersObject.transform.GetChild(0).GetComponent<Text>().text = "SRNN layers: " + (_rnnSimpleLayers).ToString();
    }
    public void OnClickGRULayer(int quantity)
    {
        if (quantity == 1)
        {
            _layersArr.Add("GRU");
            SetLayersText();
        }
        else
        {
            ResetLayers();
        }
        _gruLayers += quantity;
        if (_gruLayers < 0)
        {
            _gruLayers = 0;
        }
        layersObject.transform.GetChild(1).GetComponent<Text>().text = "GRU layers: " + (_gruLayers).ToString();
    }
    public void OnClickLSTMLayer(int quantity)
    {
        if (quantity == 1)
        {
            _layersArr.Add("LSTM");
            SetLayersText();
        }
        else
        {
            ResetLayers();
        }
        _lstmLayers += quantity;
        if(_lstmLayers < 0)
        {
            _lstmLayers = 0;
        }
        layersObject.transform.GetChild(2).GetComponent<Text>().text = "LSTM layers: " + (_lstmLayers).ToString();
    }

    public void ResetLayers()
    {
        _layersArr.Clear();
        _rnnSimpleLayers = 0;
        _gruLayers = 0;
        _lstmLayers = 0;
        layersObject.transform.GetChild(0).GetComponent<Text>().text = "SRNN layers: " + (_rnnSimpleLayers).ToString();
        layersObject.transform.GetChild(1).GetComponent<Text>().text = "GRU layers: " + (_gruLayers).ToString();
        layersObject.transform.GetChild(2).GetComponent<Text>().text = "LSTM layers: " + (_lstmLayers).ToString();
        layersText.GetComponent<Text>().text = "LAYERS: ";
    }

    public void SetLayersText()
    {
        string textLayers;
        textLayers = layersText.text = "LAYERS: ";
        for (int i = 0; i < _layersArr.Count; i++)
        {
            textLayers += " " + _layersArr[i];
        }
        layersText.text = textLayers;
    }

    //NGRAMS

    public void SetNGramsInput()
    {
        _nGramsInput = nGramsObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetLengthInput()
    {
        _lengthInput = nGramsObject.transform.GetChild(1).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    public void SetFileNameInputNGRAMS()
    {
        _fileNameNGrams = nGramsObject.transform.GetChild(2).GetChild(0).GetComponentInChildren<InputField>().text;
    }
    public void OnClickContinueNGrams()
    {
        //ADD LAYERS INPUT
        if (CheckIfCanContinue(nGramsObject))
        {
            nGramsObject.SetActive(false);
            nGramsContinue.SetActive(false);
            SetNFilesInput();
        }
    }

    private bool CheckIfCanContinue(GameObject obj)
    {
        bool canContinue = true;
        //-2 because of buttons
        for (int i = 0; i < obj.transform.childCount - 2; i++)
        {
            if (obj.transform.GetChild(i).GetChild(0).GetComponentInChildren<InputField>().text == "")
            {
                canContinue = false;
                break;
            }
        }
        return canContinue;
    }

    public string SendCommand()
    {
        int nFiles = GetFilesSelectedLength();
        string concat = "";
        for (int i = 0; i < nFiles; i++)
        {
            string file = GetFilesToConcatInput()[i];
            string sufix = ".csv";
            string concatS = file + sufix;
            concat = concat + " " + concatS;
        }
        string concatLayers = "";
        for(int i = 0; i < _layersArr.Count; i++)
        {
            string file = _layersArr[i];
            string concatS = file;
            concatLayers = concatLayers + " " + concatS;
        }
        string debug = "";
        bool debugMode = GetCheckBoxActive();
        if (debugMode)
        {
            debug = " -d";
        }
        string command = "";
        if (_mlMode)
        {
            command = "/C python NGrams.py " + nFiles.ToString() + concat + " " + _nGramsInput + " " + _lengthInput + " " + _fileNameNGrams + ".csv " + debug;
        }
        else
        {
            //"python NeuralNetworks.py 70 1-1.csv 10000 512 1024 70 0.5 LSTM_UNITY_1.csv"
            command = "/C python NeuralNetworks.py " + nFiles.ToString() + concat/*ESTO ES PARA LO DE MEZCLA DE ARCHIVOS nFiles.ToString() + concat +*/ + " " + _seqLengthInput + " " + _batchSize + " " + _bufferSizeInput + " " + _embedDimInput + " " + _nnUnitsInput + " " + _epochsInput + " "
               + _layersArr.Count + " " + concatLayers + " " + _temperatureInput + " " + _width + " " + _fileNameRNN + ".csv " + debug;
        }
        return command;
    }
}
