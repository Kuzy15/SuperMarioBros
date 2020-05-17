using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents a mushroom. A mushroom makes Mario bigger when colliding with him.
/// </summary>
public class Mushroom : MonoBehaviour
{
    //Start position of the mushroom
    private Vector3 _startPosition;
    //Move direction of the mushroom
    private int _dir;
    //Rigidbody of the mushroom
    private Rigidbody _rigidbody;
    //Bool to check if the mushroom can move
    private bool _canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        MoveMushroom();
    }

    /// <summary>
    /// This method allows a mushroom to move up until it is out of the magic block. Once is out, it check the position of the player. 
    /// It goes to the opposite direction of the player.
    /// </summary>
    private void MoveMushroom()
    {
        if (!_canMove)
        {
            _rigidbody.useGravity = false;
            _rigidbody.detectCollisions = false;
            float step = 2f * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, _startPosition + new Vector3(0, 1f), step);
            if (Vector3.Distance(transform.position, _startPosition + new Vector3(0, 1f)) < 0.001f)
            {
                // Swap the position of the cylinder.
                _canMove = true;
                if (this.transform.position.x < GameObject.Find("Mario").transform.position.x)
                {
                    _dir = -1;
                }
                else
                {
                    _dir = 1;
                }
                _rigidbody.useGravity = true;
                _rigidbody.detectCollisions = true;
            }
        }
        else
        {
            this.transform.Translate(new Vector3(2f * _dir * Time.deltaTime, _rigidbody.velocity.y * Time.deltaTime, 0));
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 0.5f) || Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 0.5f))
            {
                if (!hit.transform.gameObject.GetComponent<Enemy>() && !hit.transform.gameObject.GetComponent<Camera>())
                {
                    // Apply the force to the rigidbody.
                    this.transform.position = new Vector3(this.transform.position.x - 0.2f * _dir, this.transform.position.y, this.transform.position.z);
                    _dir *= -1;
                }
            }
        }
    }
}
