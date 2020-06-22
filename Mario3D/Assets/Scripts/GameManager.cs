using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;


    public enum SceneFlow
    {
        PREVIOUS, CURRENT, NEXT, INFO, MENU
    }

    private int _sceneIndex = 0;
    private SceneFlow _sceneFlow;
    // Start is called before the first frame update
    void Awake()
    {
        Screen.SetResolution(1024, 768, false);
        DontDestroyOnLoad(this);
        if (GM == null)
            GM = this;
        else
        {
            //DestroyMap();
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_sceneIndex != 2)
            {
                ChangeScene(SceneFlow.PREVIOUS);
            }
            else
            {
                ChangeScene(SceneFlow.MENU);
            }
        }
    }

    public void ChangeScene(SceneFlow sFlow)
    {
        switch (sFlow)
        {
            case SceneFlow.PREVIOUS:
               
                if(_sceneIndex - 1 >= 0)
                {
                    _sceneIndex--;
                }
                else
                {
                    Application.Quit();
                }
                break;
            case SceneFlow.CURRENT:
                break;
            case SceneFlow.NEXT:
                _sceneIndex++;
                break;
            case SceneFlow.INFO:
                _sceneIndex = 2;
                break;
            case SceneFlow.MENU:
                _sceneIndex = 0;
                break;
        }
        if(_sceneIndex >= 0){ 
            SceneManager.LoadScene(_sceneIndex);
        }
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
