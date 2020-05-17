using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that allows to create on the gameplay scene the map previously read
/// </summary>
public class MapLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        MapReader.GM.GenerateMap();
    }
}
