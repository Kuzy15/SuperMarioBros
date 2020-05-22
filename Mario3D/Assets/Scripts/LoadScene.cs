using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public static LoadScene Instance { get; private set; }
    private bool _change = true;
    private bool _start = false;
    private bool _canChangeScene = false;
    private string _scene;

    public GameObject image;

    private void Awake()
    {
        LoadSceneSingleton();
    }

    private void LoadSceneSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            image.SetActive(false);
        }
        /*else
        {
            Destroy(gameObject);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (_start)
        {
            if (_change)
            {
                Color tmp = image.GetComponentInChildren<UnityEngine.UI.Image>().color;
                tmp.a += 0.03f;
                image.GetComponentInChildren<UnityEngine.UI.Image>().color = tmp;
                Debug.Log("ALPHA: " + image.GetComponentInChildren<UnityEngine.UI.Image>().color.a);
                if (image.GetComponentInChildren<UnityEngine.UI.Image>().color.a > 1.5)
                {
                    _start = false;
                }
            }
            else
            {
                // Debug.Log("AA");
                Color tmp = image.GetComponentInChildren<UnityEngine.UI.Image>().color;
                tmp.a -= 0.01f;
                image.GetComponentInChildren<UnityEngine.UI.Image>().color = tmp;
                if (tmp.a <= 0)
                {
                    _start = false;
                    image.gameObject.SetActive(false);
                }
            }
        }
        Debug.Log("START: " + _start + "       CHANGESCENE: " + _canChangeScene);
        if(!_start && _canChangeScene)
        {
            SceneManager.LoadScene(_scene);
            _canChangeScene = false;
        }
    }

    public void StartFadeIn(string scene)
    {
        image.gameObject.SetActive(true);
        _start = true;
        _change = true;
        _scene = scene;
    }

    public void StartFadeOut()
    {
        image.gameObject.SetActive(true);
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
}
