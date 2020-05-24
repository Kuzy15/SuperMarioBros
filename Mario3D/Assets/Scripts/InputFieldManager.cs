using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Class used for getting and managing all the inputs on the intro scene
/// </summary>
public class InputFieldManager : MonoBehaviour
{
    //Instance of the class
    public static InputFieldManager GM;
    //Scroll of the .csv files
    public GameObject filesScrollView;
    //Object with the python thread
    public PythonThread testObject;
    //Continue button
    public GameObject continueButton;
    //Check box to set debug logs or not
    public GameObject checkBox;
    //Load one single map button
    public GameObject loadButton;
    //Generate button
    public GameObject generateButton;
    //Button for NGrams mode
    public GameObject nGramsMode;
    //Button for rnn mode
    public GameObject rnnMode;
    //Array containing all .csv on a determined path
    public List<string> arrFiles = new List<string>();

    //Fields related to all the rnn inputs
    public GameObject rnnObject;
    public GameObject rnnContinue;
    public GameObject continueScroll;
    public GameObject nGramsObject;
    public GameObject nGramsContinue;
    public GameObject layersObject;
    public Text modeText;
    public Text layersText;
    public GameObject resetLayers;

    //String containing all the selected files
    private string _nFilesInput;
    //Checks if the debug log checkbox is marked or not
    private bool _checkBoxActive;
    //*.csv path
    private string _path;
    //Save al the files
    private string[] _files;
    //Save al the file names
    private string[] _fileNames;
    private int n = 0;
    //Controls the current mode (Load or generation)
    private bool _generationMode;
    //Controls the current machine learning model (NGRAMS or RNN)
    private bool _mlMode;

    //RNN inputs (Related to rnn object). Variables used on the NeuralNetworks.py
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

    //NGRAMS. Variables used on the NGrams.py
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
        //_checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
        checkBox.SetActive(false);
        n = 0;
        GetFileNames();
    }

    /// <summary>
    /// Starts NGRAMS mode
    /// </summary>
    public void StartNGrams()
    {
        nGramsObject.SetActive(true);
    }

    /// <summary>
    /// Starts RNN mode
    /// </summary>
    public void StartRNN()
    {
        rnnObject.SetActive(true);
    }

    /// <summary>
    /// Displays the scroll view with all the files
    /// </summary>
    public void SetNFilesInput()
    {
        filesScrollView.SetActive(true);
        PopulateGrid.GM.Populate(GetFilesLength(), GetFilesNames());
        continueButton.SetActive(true);
        checkBox.SetActive(true);
    }

    /// <summary>
    /// Get all the files selected on a list
    /// </summary>
    /// <returns></returns>
    public List<string> GetFilesToConcatInput()
    {
        return arrFiles;
    }

    /// <summary>
    /// Getter of all the .csv files on ./Resources/Maps path
    /// </summary>
    public void GetFileNames()
    {
        _path = Application.streamingAssetsPath + "/Maps";
        _files = System.IO.Directory.GetFiles(_path, "*.csv");
        _fileNames = new string[_files.Length];
        for (int i = 0; i < _files.Length; i++)
        {
            string file = System.IO.Path.GetFileNameWithoutExtension(_files[i]);
            _fileNames[i] = file;
        }
    }

    /// <summary>
    /// Getter of files names
    /// </summary>
    /// <returns></returns>
    public string[] GetFilesNames()
    {
        return _fileNames;
    }

    /// <summary>
    /// Getter of files array length
    /// </summary>
    /// <returns></returns>
    public int GetFilesLength()
    {
        return _files.Length;
    }

    /// <summary>
    /// This method add a single file to the selected files array
    /// </summary>
    /// <param name="item"></param>
    public void AddFile(string item)
    {
        filesScrollView.transform.GetChild(0).GetComponent<Text>().text += " " + item;
        arrFiles.Add(item);
    }

    /// <summary>
    /// This method removes a single file of the selected files array
    /// </summary>
    /// <param name="item"></param>
    public void RemoveFile(string item)
    {
        arrFiles.RemoveAt(arrFiles.IndexOf(item));
        ResetText();
    }

    /// <summary>
    /// This method resets selected files text without the previous item removed
    /// </summary>
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

    /// <summary>
    /// Getter of the selected files array length
    /// </summary>
    /// <returns></returns>
    public int GetFilesSelectedLength()
    {
        return arrFiles.Count;
    }
    
    /// <summary>
    /// Getter of the value of the debug log check box
    /// </summary>
    /// <returns></returns>
    public bool GetCheckBoxActive()
    {
        return _checkBoxActive;
    }

    /// <summary>
    /// Button method to continue to the next step of the input.
    /// If load mode goes to the file selected.
    /// If generation mode, calls the python thread to execute the command with the
    /// proper variables
    /// </summary>
    public void OnClickContinue()
    {
        string text = " ";
        if (_generationMode)
        {
            string fileToGen = "";
            if (_mlMode)
            {
                text = "GENERATING NGRAMS";
                fileToGen = _fileNameNGrams;
            }
            else
            {
                text = "GENERATING RNN";
                fileToGen = _fileNameRNN;
            }
            if (arrFiles.Count > 0)
            {
                LoadScene.Instance.ActiveLoadObject();
                LoadScene.Instance.SetLoadText(text);
                LoadScene.Instance.StartFadeIn("SampleScene");
                StartCoroutine(StartGenCoroutine(fileToGen));
                
            }
        }
        else
        {
            if (arrFiles.Count == 1)
            {
                text = "LOADING";
                //PythonThread.ExecuteCommand();
                LoadScene.Instance.ActiveLoadObject();
                LoadScene.Instance.SetLoadText(text);
                LoadScene.Instance.StartFadeIn("SampleScene");
                MapReader.GM.InitMap(arrFiles[0], false);
                LoadScene.Instance.ChangeScene();
            }
        }
    }

    private IEnumerator StartGenCoroutine(string fileToGen)
    {
        yield return new WaitForSeconds(1.5f);
        LoadScene.Instance.ChangeScene();
        PythonThread.ExecuteCommand();
        MapReader.GM.InitMap(fileToGen);
    }

    /// <summary>
    /// Method assigned to a checkbox object to get if it is marked or not
    /// </summary>
    public void OnCheckBoxValueChanged()
    {
        _checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
        Debug.Log(_checkBoxActive);
    }

    /// <summary>
    /// Setter of the current mode (Generation or Load)
    /// </summary>
    /// <param name="generation"></param>
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

    /// <summary>
    /// Enters to the generating mode
    /// </summary>
    public void OnGeneratingMode()
    {
        nGramsMode.SetActive(true);
        rnnMode.SetActive(true);
        //nGramsField.SetActive(true);
        /*SetNGramsInput();
        SetLengthInput();
        SetNFilesInput();*/
    }

    /// <summary>
    /// Enters to the loading mode
    /// </summary>
    public void OnLoadMode()
    {
        SetNFilesInput();
    }

    /// <summary>
    /// Getter of the current mode (generation or load)
    /// </summary>
    /// <returns></returns>
    public bool GetGenerationMode()
    {
        return _generationMode;
    }


    /// <summary>
    /// Setter of the machine learning mode (NGRAMS or RNN)
    /// </summary>
    /// <param name="nGrams"></param>
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

    /// <summary>
    /// Method assigned to a button to reload the scene
    /// </summary>
    public void BackButton()
    {
        GameManager.GM.ChangeScene(GameManager.SceneFlow.CURRENT);
    }


    //RNN
    /// <summary>
    /// Setter of sequence length(RNN)
    /// </summary>
    public void SetSeqLengthInput()
    {
        _seqLengthInput = rnnObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of buffer size(RNN)
    /// </summary>
    public void SetBufferSizeInput()
    {
        _bufferSizeInput = rnnObject.transform.GetChild(1).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of embedding dimension(RNN)
    /// </summary>
    public void SetEmbedDimInput()
    {
        _embedDimInput = rnnObject.transform.GetChild(2).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of neural network units(RNN)
    /// </summary>
    public void SetNNUnitsInput()
    {
        _nnUnitsInput = rnnObject.transform.GetChild(3).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of training epochs(RNN)
    /// </summary>
    public void SetEpochsInput()
    {
        _epochsInput = rnnObject.transform.GetChild(4).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of temperature (Randomness)(RNN)
    /// </summary>
    public void SetTemperatureInput()
    {
        _temperatureInput = rnnObject.transform.GetChild(5).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of the file name to create(RNN)
    /// </summary>
    public void SetFileNameInput()
    {
        _fileNameRNN = rnnObject.transform.GetChild(6).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of the batch size(RNN)
    /// </summary>
    public void SetBatchSize()
    {
        _batchSize = rnnObject.transform.GetChild(7).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of map width(RNN)
    /// </summary>
    public void SetWidth()
    {
        _width = rnnObject.transform.GetChild(8).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Method assigned to a button, to continue to the next step(RNN)
    /// </summary>
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

    /// <summary>
    /// Method assigned to a button, to open scroll files(RNN)
    /// </summary>
    public void OnClickContinueScroll()
    {
        //ADD LAYERS INPUT
            layersObject.SetActive(false);
            SetNFilesInput();
    }

    /// <summary>
    /// Setter of number of RNNSimple layers to use(RNN)
    /// </summary>
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

    /// <summary>
    /// Setter of number of GRU layers to use(RNN)
    /// </summary>
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

    /// <summary>
    /// Setter of number of LSTM layers to use(RNN)
    /// </summary>
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


    /// <summary>
    /// Resets all the layers to use
    /// </summary>
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

    /// <summary>
    /// Setter of layers to use text(RNN)
    /// </summary>
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
    /// <summary>
    /// Setter of ngrams (1-gram, 2-gram, 3-gram,etc.)(NGRAMS)
    /// </summary>
    public void SetNGramsInput()
    {
        _nGramsInput = nGramsObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of map width(NGRAMS)
    /// </summary>
    public void SetLengthInput()
    {
        _lengthInput = nGramsObject.transform.GetChild(1).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of file name to create(NGRAMS)
    /// </summary>
    public void SetFileNameInputNGRAMS()
    {
        _fileNameNGrams = nGramsObject.transform.GetChild(2).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Method assigned to a button, to continue to the next step(NGRAMS)
    /// </summary>
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


    /// <summary>
    /// This method checks if all input fields have a value in order to continue to the next step
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Sends an specific command depending on the machine learning mode selected. Gets all the machine learning mode values and concats them on an string.
    /// </summary>
    /// <returns></returns>
    public string SendCommand()
    {
        int nFiles = GetFilesSelectedLength();
        string concat = "";
        for (int i = 0; i < nFiles; i++)
        {
            string file = GetFilesToConcatInput()[i];
            string sufix = ".csv";
            string concatS = "..\\Maps\\" + file + sufix;
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
            command = "/C python NGrams.py " + nFiles.ToString() + concat + " " + _nGramsInput + " " + _lengthInput + " " +  "..\\Maps\\" + _fileNameNGrams + ".csv " + debug;
        }
        else
        {
            //"python NeuralNetworks.py 70 1-1.csv 10000 512 1024 70 0.5 LSTM_UNITY_1.csv"
            command = "/C python NeuralNetworks.py " + nFiles.ToString() + concat/*ESTO ES PARA LO DE MEZCLA DE ARCHIVOS nFiles.ToString() + concat +*/ + " " + _seqLengthInput + " " + _batchSize + " " + _bufferSizeInput + " " + _embedDimInput + " " + _nnUnitsInput + " " + _epochsInput + " "
               + _layersArr.Count + " " + concatLayers + " " + _temperatureInput + " " + _width + " " + "..\\Maps\\" + _fileNameRNN + ".csv " + debug;
        }
        return command;
    }
}
