using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public static LoadScene Instance { get; private set; }
    public GameObject image;
    public MoveImage moveImage;

    private bool _change = true;
    private bool _start = false;
    private bool _canChangeScene = false;
    private bool _coroutine = false;
    private string _scene;
    private GameObject _internalImage;
    private GameObject _moveImage;
    private string _fileToGen = "";
    private string _text;

    private bool _resetText = false;

    private List<string> _commands;
    private bool _mlMode;


    private void Awake()
    {
        LoadSceneSingleton();
    }

    private void LoadSceneSingleton()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            //DestroyMap();
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _internalImage = Instantiate(image, this.gameObject.transform);
        _moveImage = Instantiate(moveImage.gameObject, this.gameObject.transform);
        /*if (image == null)
        {
            image = GameObject.Find("CanvasLoad");
        }*/
        _internalImage.SetActive(false);
        this.transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "LOADING";
        //_moveImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_start)
        {
            if (_moveImage.GetComponent<MoveImage>().HasFinishedMovement())
            {
                if (_change)
                {
                    if (!_resetText)
                    {
                        this.transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Text>().text = _text;
                        _resetText = true;
                    }
                    Color tmp2 = _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color;
                    tmp2.a += 0.08f;
                    _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color = tmp2;
                    if(_internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color.a > 1.5f)
                    {
                        _start = false;
                        if (!_coroutine && _fileToGen != "")
                        {
                            if (_commands.Count > 1)
                            {
                                PythonThread.ExecuteCommand(_commands[1]);
                                //PythonThread.ExecuteCommand("/C python GenerateNGrams.py ..\\NGramsTraining\\20200528_112114_Training_Ngrams.pkl 250 ..\\Maps\\25.csv");
                            }
                            StartCoroutine(StartGenCoroutine(_fileToGen));
                            _coroutine = false;
                        }
                    }
                    /*if (!_resetText && _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color.a > 0)
                    {
                        Color tmp = _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color;
                        tmp.a -= 0.01f;
                        _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color = tmp;
                        Debug.Log("ALPHA: " + _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color.a);
                    }
                    if (!_resetText && _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color.a <= 0)
                    {
                        _start = false;
                        Color tmp2 = _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color;
                        tmp2.a = 0f;
                        _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color = tmp2;
                        _resetText = true;
                        SetLoadText(_text);
                    }
                    else
                    {
                        Color tmp2 = _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color;
                        tmp2.a += 0.01f;
                        _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color = tmp2;
                        if (_internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color.a > 1.5f && _fileToGen != "")
                    }*/
                }
                else
                {
                    // Debug.Log("AA");
                    Color tmp = _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color;
                    tmp.a -= 0.01f;
                    _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color = tmp;
                    if (tmp.a <= 0)
                    {
                        _start = false;
                        _internalImage.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Color tmp = _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color;
                tmp.a -= 0.01f;
                _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color = tmp;
                if (_internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color.a <= 0)
                {
                }
            }
        }
        //Debug.Log("START: " + _start + "       CHANGESCENE: " + _canChangeScene);
        if (!_start && _canChangeScene && _moveImage.GetComponent<MoveImage>().CanStartLoading())
        {
            GameManager.GM.ChangeScene(GameManager.SceneFlow.NEXT);
            _canChangeScene = false;
            //GameCamera.Instance.GoToBlueScreen();
        }
    }

    public void StartFadeIn(string scene, string fileToGen)
    {
        /*if(_internalImage == null)
        {
            image = GameObject.Find("CanvasLoad");
        }*/
        Color tmp2 = _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color;
        tmp2.a = 1f;
        _internalImage.transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Text>().color = tmp2;
        _resetText = false;
        this.transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "LOADING";
        CanvasActivator.Instance.ActivateCanvas(false);
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = Color.black;
        _moveImage.SetActive(true);
        _moveImage.GetComponent<MoveImage>().ActiveImage();
        _internalImage.gameObject.SetActive(true);
        _start = true;
        _change = true;
        _scene = scene;
        _fileToGen = fileToGen;
    }

    public void StartFadeOut()
    {
        /*if (image == null)
        {
            image = GameObject.Find("CanvasLoad");
        }*/
        _moveImage.gameObject.SetActive(false);
        _internalImage.gameObject.SetActive(true);
        _start = true;
        _change = false;
    }

    private IEnumerator StartGenCoroutine(string fileToGen)
    {
        yield return new WaitForSeconds(1.5f);
        LoadScene.Instance.ChangeScene();
        UpdateTrainingCommand();
        PythonThread.ExecuteCommand(_commands[0]);
        MapReader.GM.InitMap(fileToGen);
    }

    public bool GetStart()
    {
        return _start;
    }

    public void ChangeScene()
    {
        _canChangeScene = true;
    }

    public void ActiveLoadObject()
    {
        this.gameObject.SetActive(true);
    }

    public GameObject CheckImageNull()
    {
        return _internalImage;
    }

    public void SetLoadText(string text)
    {
        _text = text;
    }

    public void GetCommands(List<string> commands)
    {
        _commands = commands;
    }

    public void GetMode(bool mlMode)
    {
        _mlMode = mlMode;
    }

    public void UpdateTrainingCommand()
    {
        string path = "";
        if (_mlMode)
        {
            path = "NGramsTraining";
        }
        else
        {
            path = "NNTraining";
        }
        string[] files = System.IO.Directory
                        .GetFiles(Application.streamingAssetsPath + "/PythonScripts/" + path, "*.pkl");
        string f = files[0];
        string file = System.IO.Path.GetFileNameWithoutExtension(files[files.Length - 1]);
        _commands[0] = _commands[0].Replace("TRAININGFILE", "..\\" + path + "\\" + file + ".pkl ");
    }
}
