using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MapReader.GM.DestroyMap();
        DontDestroyObject[] allObjects = UnityEngine.Object.FindObjectsOfType<DontDestroyObject>();

        foreach (DontDestroyObject gos in allObjects)
        {
            if (gos.gameObject.activeInHierarchy == true)
            {
                Destroy(gos);
            }
        }
    }
}
