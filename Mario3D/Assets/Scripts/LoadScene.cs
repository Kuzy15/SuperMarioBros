using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public static LoadScene Instance { get; private set; }
    public GameObject image;

    private bool _change = true;
    private bool _start = false;
    private bool _canChangeScene = false;
    private string _scene;
    private GameObject _internalImage;
    

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
        /*if (image == null)
        {
            image = GameObject.Find("CanvasLoad");
        }*/
        _internalImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_start)
        {
            if (_change)
            {
                Color tmp = _internalImage.GetComponentInChildren<UnityEngine.UI.Image>().color;
                tmp.a += 0.03f;
                _internalImage.GetComponentInChildren<UnityEngine.UI.Image>().color = tmp;
                Debug.Log("ALPHA: " + _internalImage.GetComponentInChildren<UnityEngine.UI.Image>().color.a);
                if (_internalImage.GetComponentInChildren<UnityEngine.UI.Image>().color.a > 1.5)
                {
                    _start = false;
                }
            }
            else
            {
                // Debug.Log("AA");
                Color tmp = _internalImage.GetComponentInChildren<UnityEngine.UI.Image>().color;
                tmp.a -= 0.01f;
                _internalImage.GetComponentInChildren<UnityEngine.UI.Image>().color = tmp;
                if (tmp.a <= 0)
                {
                    _start = false;
                    _internalImage.gameObject.SetActive(false);
                }
            }
        }
        Debug.Log("START: " + _start + "       CHANGESCENE: " + _canChangeScene);
        if(!_start && _canChangeScene)
        {
            GameManager.GM.ChangeScene(GameManager.SceneFlow.NEXT);
            _canChangeScene = false;
        }
    }

    public void StartFadeIn(string scene)
    {
        /*if(_internalImage == null)
        {
            image = GameObject.Find("CanvasLoad");
        }*/
        _internalImage.gameObject.SetActive(true);
        _start = true;
        _change = true;
        _scene = scene;
    }

    public void StartFadeOut()
    {
        /*if (image == null)
        {
            image = GameObject.Find("CanvasLoad");
        }*/
        _internalImage.gameObject.SetActive(true);
        _start = true;
        _change = false;
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
        this.transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Text>().text = text;
    }
}
