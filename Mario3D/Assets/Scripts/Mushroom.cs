using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    private int _currentAnim;
    private SpriteRenderer _renderer;
    private Vector3 _startPosition;
    private int _dir;
    private Rigidbody _rigidbody;
    private bool _spawning = true;
    private float speed = 3;
    private bool _canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _rigidbody = this.GetComponent<Rigidbody>();
        _startPosition = transform.position;
        //_rigidbody.AddForce(new Vector3(0, 4f), ForceMode.Impulse);
        //Physics.gravity = new Vector3(0, -9 * _rigidbody.mass, 0);

    }

    private void FixedUpdate()
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
        else {
            this.transform.Translate(new Vector3(2f * _dir * Time.deltaTime, _rigidbody.velocity.y * Time.deltaTime, 0));
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 0.5f) || Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 0.5f))
            {
                // Apply the force to the rigidbody.
                this.transform.position = new Vector3(this.transform.position.x - 0.2f * _dir, this.transform.position.y, this.transform.position.z);
                _dir *= -1;
            }
        }
        
        /*
        this.transform.Translate(new Vector3(2f * _dir * Time.deltaTime, _rigidbody.velocity.y*Time.deltaTime, 0));
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 0.6f))
        {
            // Apply the force to the rigidbody.
            this.transform.position = new Vector3(this.transform.position.x - 0.2f * _dir, this.transform.position.y, this.transform.position.z);
            _dir *= -1;
        }*/
        //Debug.Log(_player.GetComponent<Player>().GetMarioPosition().x + "    " + this.transform.position.x);
    }

    // Update is called once per frame
   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Growup
            Destroy(this.gameObject);
        }
        /*else
        {
            Debug.Log("Colliding");
            _rigidbody.velocity = new Vector3(2f * _dir, _rigidbody.velocity.y, 0);
        }*/
        /*else if (collision.gameObject.tag == "Solid" /*&& !_canMove)
        {
            Debug.Log("SOLID");
            _canMove = true;
            _rigidbody.AddForce(new Vector3(4f, 0f)*Time.deltaTime, ForceMode.Impulse);
        }*/
    //}*/


}
