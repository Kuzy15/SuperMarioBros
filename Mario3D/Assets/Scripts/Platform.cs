using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that represents a single platform. It moves on a sin movement.
/// </summary>
public class Platform : MonoBehaviour
{
    //Start position of the platform
    private Vector3 _startPosition;

    // Start is called before the first frame update
    private void Start()
    {
        _startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, _startPosition.y + Mathf.Sin(Time.time * 2f) - 0.75f, 0);
    }
}
