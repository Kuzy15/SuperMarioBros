using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCastle : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject _player;
    private GameObject castleDoor;

    void Start()
    {
        castleDoor = GameObject.FindGameObjectWithTag("CastleDoor");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("CASTLE: " + this.transform.localPosition.x + "       " + castleDoor.transform.localPosition.x);
        if(this.transform.localPosition.x >= castleDoor.transform.localPosition.x)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
        }
    }
}
