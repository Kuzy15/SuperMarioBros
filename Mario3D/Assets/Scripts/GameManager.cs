using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;


    public enum SceneFlow
    {
        PREVIOUS, CURRENT, NEXT
    }

    private int _sceneIndex = 0;
    private SceneFlow _sceneFlow;
    // Start is called before the first frame update
    void Awake()
    {
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
            ChangeScene(SceneFlow.PREVIOUS);
        }
    }

    public void ChangeScene(SceneFlow sFlow)
    {
        switch (sFlow)
        {
            case SceneFlow.PREVIOUS:
                _sceneIndex--;
                break;
            case SceneFlow.CURRENT:
                break;
            case SceneFlow.NEXT:
                _sceneIndex++;
                break;
        }
        if(_sceneIndex >= 0){ 
            SceneManager.LoadScene(_sceneIndex);
        }
        else
        {
            Application.Quit();
        }
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
