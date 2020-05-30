using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasActivator : MonoBehaviour
{

    public static CanvasActivator Instance { get; private set; }

    private void Awake()
    {
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

    public void ActivateCanvas(bool act)
    {
        this.gameObject.SetActive(act);
    }
}
