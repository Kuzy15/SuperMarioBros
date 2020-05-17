using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents a brick that can be broken on collision
/// </summary>

public class BreakableBrick : MonoBehaviour
{
    /// <summary>
    /// Function used for destroyin the gameobject that containts this class (A brick)
    /// </summary>
    public void DestroyBrick()
    {
        Destroy(this.gameObject);
    }
}
