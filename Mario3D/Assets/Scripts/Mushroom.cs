using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    private float _time;
    private int _currentAnim;
    private SpriteRenderer _renderer;
    private Vector2 _startPosition;
    private int _dir = 1;
    private Rigidbody _rigidbody;

    public float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveMushroom();
        //RaycastHit2D hit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), 0.1f);
        //Ray2D ray = new Ray2D(transform.position, mouseDirection);
        /*Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right), Color.white);
        // If it hits something...
        //Debug.DrawRay(this.transform.position, hit.transform.position);*/
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 0.6f))
        {
             // Apply the force to the rigidbody.
             _dir *= -1;
        }
    }

    private void MoveMushroom()
    {
        transform.Translate(_dir * Time.deltaTime * speed, 0, 0);
        //_rigidbody.velocity = new Vector2 (_dir * speed, 0);
    }

    /*private void AnimMushroom()
    {
        _time += Time.deltaTime * animSpeed;
        if (_time >= 1.0f)
        {
            _currentAnim++;
            _time = 0;
        }

        if (_currentAnim < anim.Length)
        {
            _renderer.sprite = anim[_currentAnim];
        }
        else
        {
            _currentAnim = 0;
        }
    }*/


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
