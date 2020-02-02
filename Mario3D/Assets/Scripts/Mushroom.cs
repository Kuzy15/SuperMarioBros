using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    private int _currentAnim;
    private SpriteRenderer _renderer;
    private Vector3 _startPosition;
    private int _dir = 1;
    private Rigidbody _rigidbody;
    private bool _spawning = true;
    private float speed = 3;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _rigidbody = this.GetComponent<Rigidbody>();
        _startPosition = transform.position;
        Physics.gravity = new Vector3(0, -9 * _rigidbody.mass, 0);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /* if (_spawning)
         {
             //transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, 1), 2 * Time.deltaTime);
             if (transform.position.y >= _startPosition.y + 1)
             {
                 _spawning = false;
             }
             else
             {
                 _rigidbody.velocity = new Vector3(0, 1, 0);
             }
         }
         else
         {*/
        _rigidbody.velocity = new Vector3(2, 0, 0);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 0.6f))
        {
            // Apply the force to the rigidbody.
            _dir *= -1;
        }
        // }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Growup
            Destroy(this.gameObject);
        }
       /* else if (collision.gameObject.tag == "Barrier")
        {
            transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z);
            _dir *= -1;
        }*/
    }


}
