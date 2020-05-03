using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBrick : MonoBehaviour
{
    public void DestroyBrick()
    {
        Destroy(this.gameObject);
    }
}
