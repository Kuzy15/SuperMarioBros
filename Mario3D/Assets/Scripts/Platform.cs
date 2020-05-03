using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Update is called once per frame
    private Vector3 _startPosition;
    private void Start()
    {
        _startPosition = this.transform.position;
    }
    void Update()
    {
        transform.position = new Vector3(transform.position.x, _startPosition.y + Mathf.Sin(Time.time * 2f) - 0.75f, 0);
    }
}
