using System.Collections;
using System.Linq;
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
    //Title
    public GameObject title;
    //Info button
    public GameObject infoButton;

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

    public enum GenerateOptions
    {
        LOAD, LOAD_TRAIN, TRAIN
    }

    private GenerateOptions _gOp;

    private List<string> _commands = new List<string>();

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
    }

    private void Update()
    {
        //Debug.Log("MLMODE: " + _mlMode);
    }

    /// <summary>
    /// Starts NGRAMS mode
    /// </summary>
    public void StartNGrams()
    {
        nGramsObject.transform.GetChild(4).gameObject.SetActive(true);
        nGramsObject.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => ButtonAction(GenerateOptions.TRAIN));
        nGramsObject.transform.GetChild(5).gameObject.SetActive(true);
        nGramsObject.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => ButtonAction(GenerateOptions.LOAD_TRAIN));
        nGramsObject.transform.GetChild(6).gameObject.SetActive(true);
        nGramsObject.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => ButtonAction(GenerateOptions.LOAD));
    }

    public void StartNGramsInput(int q)
    {
        for (int i = 0; i < q; i++)
        {
            nGramsObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        if (nGramsObject.transform.GetChild(3).gameObject.activeSelf == false)
            nGramsObject.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void DeactivateNgramsField(int i)
    {
        nGramsObject.transform.GetChild(i).gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts RNN mode
    /// </summary>
    public void StartRNN()
    {
        //rnnObject.SetActive(true);
        rnnObject.transform.GetChild(10).gameObject.SetActive(true);
        rnnObject.transform.GetChild(10).GetComponent<Button>().onClick.AddListener(() => ButtonAction(GenerateOptions.TRAIN));
        rnnObject.transform.GetChild(11).gameObject.SetActive(true);
        rnnObject.transform.GetChild(11).GetComponent<Button>().onClick.AddListener(() => ButtonAction(GenerateOptions.LOAD_TRAIN));
        rnnObject.transform.GetChild(12).gameObject.SetActive(true);
        rnnObject.transform.GetChild(12).GetComponent<Button>().onClick.AddListener(() => ButtonAction(GenerateOptions.LOAD));
    }

    public void StartRNNInputs(int q, int i = 0)
    {
        for (int j = i; j < q; j++)
        {
            rnnObject.transform.GetChild(j).gameObject.SetActive(true);
        }
        /*if (rnnObject.transform.GetChild(3).gameObject.activeSelf == false)
            rnnObject.transform.GetChild(3).gameObject.SetActive(true);*/
    }

    public void StartRNNInput(int i)
    {
        rnnObject.transform.GetChild(i).gameObject.SetActive(true);
    }

    public void DeactivateRNNField(int i)
    {
        rnnObject.transform.GetChild(i).gameObject.SetActive(false);
    }

    public void DeactivateRNNFields(int i)
    {
        for (int j = 0; j < i; j++)
        {
            rnnObject.transform.GetChild(j).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Displays the scroll view with all the files
    /// </summary>
    public void SetNFilesInput()
    {
        GetFileNames();
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
        if (!_generationMode)
        {
            _path = Application.streamingAssetsPath + "/Maps";
            _files = System.IO.Directory.GetFiles(_path, "*.csv");
        }
        else
        {
            switch (_gOp)
            {
                case GenerateOptions.LOAD:
                    string trainPath = "";
                    if (_mlMode)
                    {
                        trainPath = "/PythonScripts/NgramsTraining";
                    }
                    else
                    {
                        trainPath = "/PythonScripts/NNTraining";
                    }
                    _path = Application.streamingAssetsPath + trainPath;
                    _files = System.IO.Directory.GetFiles(_path, "*.pkl");
                    break;
                case GenerateOptions.LOAD_TRAIN:
                case GenerateOptions.TRAIN:
                    _path = Application.streamingAssetsPath + "/Maps";
                    _files = System.IO.Directory.GetFiles(_path, "*.csv");
                    break;
            }
        }
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
                fileToGen = _fileNameNGrams;
                text = "GENERATING NGRAMS";
            }
            else
            {
                fileToGen = _fileNameRNN;
                text = "GENERATING RNN";
            }
            if (arrFiles.Count > 0)
            {
                if (_gOp != GenerateOptions.TRAIN)
                {
                    LoadScene.Instance.ActiveLoadObject();
                    SendCommand();
                    LoadScene.Instance.GetCommands(_commands);
                    LoadScene.Instance.SetLoadText(text);
                    LoadScene.Instance.StartFadeIn("SampleScene", fileToGen);
                }
                else
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

                    string debug = "";
                    bool debugMode = GetCheckBoxActive();
                    if (debugMode)
                    {
                        debug = " -d";
                    }
                    if (_mlMode)
                    {
                        PythonThread.ExecuteCommand("/C python TrainNGrams.py " + nFiles.ToString() + concat + " " + _nGramsInput + " " + debug);
                    }
                    else
                    {
                        string concatLayers = "";
                        for (int i = 0; i < _layersArr.Count; i++)
                        {
                            string file = _layersArr[i];
                            string concatS = file;
                            concatLayers = concatLayers + " " + concatS;
                        }
                        PythonThread.ExecuteCommand("/C python TrainNN.py " + nFiles.ToString() + concat/*ESTO ES PARA LO DE MEZCLA DE ARCHIVOS nFiles.ToString() + concat +*/ + " " + _seqLengthInput + " " + _batchSize + " " + _bufferSizeInput + " " + _embedDimInput + " " + _nnUnitsInput + " " + _epochsInput + " "
            + _layersArr.Count + " " + concatLayers + " " + _temperatureInput + " " + debug);
                    }
                    GameManager.GM.ChangeScene(GameManager.SceneFlow.CURRENT);
                }

            }
        }
        else
        {
            if (arrFiles.Count == 1)
            {
                text = "MAP: " + arrFiles[0];
                //PythonThread.ExecuteCommand();
                LoadScene.Instance.ActiveLoadObject();
                LoadScene.Instance.SetLoadText(text);
                LoadScene.Instance.StartFadeIn("SampleScene", "");
                MapReader.GM.InitMap(arrFiles[0], false);
                LoadScene.Instance.ChangeScene();
            }
        }
    }



    /// <summary>
    /// Method assigned to a checkbox object to get if it is marked or not
    /// </summary>
    public void OnCheckBoxValueChanged()
    {
        _checkBoxActive = checkBox.GetComponent<Toggle>().IsActive();
        //Debug.Log(_checkBoxActive);
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
        title.SetActive(false);
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
    /// Setter of the batch size(RNN)
    /// </summary>
    public void SetBatchSize()
    {
        _batchSize = rnnObject.transform.GetChild(6).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of map width(RNN)
    /// </summary>
    public void SetWidth()
    {
        _width = rnnObject.transform.GetChild(7).GetChild(0).GetComponentInChildren<InputField>().text;
    }

    /// <summary>
    /// Setter of the file name to create(RNN)
    /// </summary>
    public void SetFileNameInput()
    {
        _fileNameRNN = rnnObject.transform.GetChild(8).GetChild(0).GetComponentInChildren<InputField>().text;
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
            if (_gOp != GenerateOptions.LOAD)
            {
                layersObject.SetActive(true);
            }
            else
            {
                SetNFilesInput();
            }
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
        if (quantity == 1)
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
        if (_lstmLayers < 0)
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
        for (int i = CheckInactiveInputs(obj, obj.transform.childCount - 4); i < obj.transform.childCount - 4; i++)
        {
            if (obj.transform.GetChild(i).gameObject.activeSelf && obj.transform.GetChild(i).GetChild(0).GetComponentInChildren<InputField>().text == "")
            {
                canContinue = false;
                break;
            }
        }
        return canContinue;
    }

    private int CheckInactiveInputs(GameObject obj, int q)
    {
        int index = 0;
        for (int i = 0; i < q; i++)
        {
            if (obj.transform.GetChild(i).gameObject.activeSelf == false)
            {
                index++;
            }
        }
        return index;
    }

    public void ButtonAction(GenerateOptions gOp)
    {
        _gOp = gOp;
        switch (gOp)
        {
            case GenerateOptions.LOAD:
                if (_mlMode)
                {
                    StartNGramsInput(4);
                    DeactivateNgramsField(0);
                }
                else
                {
                    StartRNNInputs(10);
                    DeactivateRNNFields(7);
                }
                //Debug.Log("LOAD MODEL");
                break;
            case GenerateOptions.LOAD_TRAIN:
                if (_mlMode)
                {
                    StartNGramsInput(4);
                }
                else
                {
                    StartRNNInputs(10);
                }
                //Debug.Log("LOADTRAIN MODEL");
                break;
            case GenerateOptions.TRAIN:
                if (_mlMode)
                {
                    StartNGramsInput(1);
                }
                else
                {
                    StartRNNInputs(7);
                    StartRNNInput(9);
                }
                //Debug.Log("TRAIN MODEL");
                break;
        }
        if (_mlMode)
        {
            DeactivateNgramsField(4);
            DeactivateNgramsField(5);
            DeactivateNgramsField(6);
        }
        else
        {
            DeactivateRNNField(10);
            DeactivateRNNField(11);
            DeactivateRNNField(12);
        }
    }

    /// <summary>
    /// Sends an specific command depending on the machine learning mode selected. Gets all the machine learning mode values and concats them on an string.
    /// </summary>
    /// <returns></returns>
    public void SendCommand()
    {
        int nFiles = GetFilesSelectedLength();
        string concat = "";
        switch (_gOp)
        {
            case GenerateOptions.LOAD:
                for (int i = 0; i < nFiles; i++)
                {
                    string file = GetFilesToConcatInput()[i];
                    string sufix = ".pkl";
                    string concatS = file + sufix;
                    concat = concat + " " + concatS;
                }
                break;
            case GenerateOptions.LOAD_TRAIN:
                for (int i = 0; i < nFiles; i++)
                {
                    string file = GetFilesToConcatInput()[i];
                    string sufix = ".csv";
                    string concatS = "..\\Maps\\" + file + sufix;
                    concat = concat + " " + concatS;
                }
                break;
        }

        string concatLayers = "";
        for (int i = 0; i < _layersArr.Count; i++)
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
        switch (_gOp)
        {
            case GenerateOptions.LOAD:
                string commandToSend = "";
                if (_mlMode)
                {
                    commandToSend = "/C python GenerateNGrams.py " + concat + " " + _lengthInput + " ..\\Maps\\" + _fileNameNGrams + ".csv" + debug;
                }
                else
                {
                    commandToSend = "/C python GenerateNN.py " + concat + " " + _width + " ..\\Maps\\" + _fileNameRNN + ".csv" + debug;
                }
                PushCommands(commandToSend);
                break;
            case GenerateOptions.LOAD_TRAIN:
                string path = "";
                if (_mlMode)
                {
                    path = "/PythonScripts/NGramsTraining";
                }
                else
                {
                    path = "/PythonScripts/NNTraining";
                }
                /*string[] files = System.IO.Directory
                    .GetFiles(Application.streamingAssetsPath + path, "*.pkl");
                string file = System.IO.Path.GetFileNameWithoutExtension(files[files.Length-1]);*/
                string commandToSend1 = "", commandToSend2 = "";
                if (_mlMode)
                {
                    commandToSend1 = "/C python GenerateNGrams.py " + "TRAININGFILE " + _lengthInput + " ..\\Maps\\" + _fileNameNGrams + ".csv" + debug;
                    commandToSend2 = "/C python TrainNGrams.py " + nFiles.ToString() + concat + " " + _nGramsInput + " " + debug;
                }
                else
                {
                    commandToSend1 = "/C python GenerateNN.py " + "TRAININGFILE " + _width + " ..\\Maps\\" + _fileNameRNN + ".csv" + debug;
                    commandToSend2 = "/C python TrainNN.py " + nFiles.ToString() + concat/*ESTO ES PARA LO DE MEZCLA DE ARCHIVOS nFiles.ToString() + concat +*/ + " " + _seqLengthInput + " " + _batchSize + " " + _bufferSizeInput + " " + _embedDimInput + " " + _nnUnitsInput + " " + _epochsInput + " "
            + _layersArr.Count + " " + concatLayers + " " + _temperatureInput + " " + debug;
                }
                PushCommands(commandToSend1);
                PushCommands(commandToSend2);
                break;
        }
        LoadScene.Instance.GetMode(_mlMode);
    }

    private void PushCommands(string command)
    {
        _commands.Add(command);
    }

    public GenerateOptions GetGenOp()
    {
        return _gOp;
    }

    public void OnInfoButton()
    {
        GameManager.GM.ChangeScene(GameManager.SceneFlow.INFO);
    }
}
