using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents an Enemy as an object
/// </summary>
public class Enemy : MonoBehaviour
{
    //Animation array of the enemy
    public Sprite[] anim;
    //Dead animation of the enemy
    public Sprite animDead;
    //Speed animation
    public float animSpeed = 4;

    //Start position of the enemy
    protected Vector3 _startPosition;
    protected Vector3 positionShell;
    //Renderer, collider and rigidbody of the enemy
    protected SpriteRenderer _renderer;
    protected BoxCollider _collider;
    protected Rigidbody _rigidbody;
    //Moving direction of the enemy
    protected int _dir = -1;
    //Moving velocity of the enemy
    protected float _velocity = 1.0f;
    //Bool to check if enemy is dead
    protected bool _dead = false;
    //Bool to check if the enemy can move
    protected bool _canMove = true;

    //Animation time
    private float _time;
    //Current frame of the animation
    private int _currentAnim = 0;



    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _collider = this.GetComponent<BoxCollider>();
        _rigidbody = this.GetComponent<Rigidbody>();
        _startPosition = this.transform.position;
        positionShell = _startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!LoadScene.Instance.GetStart() || !GameCamera.Instance.GetLooking())
        {
            bool vis = GameCamera.Instance.IsVisibleFrom(_renderer, Camera.main);
            if (vis)
            {
                _canMove = true;
            }
            else
            {
                _canMove = false;
            }


            if (!_dead)
            {
                EnemyAnim();
            }
        }
    }


    private void FixedUpdate()
    {
        // si esta dentro del rango de camara
        if (!GameCamera.Instance.GetLooking() && _canMove && !_dead)
        {
            EnemyMove();
        }
    }


    /// <summary>
    /// Class that animates an enemy
    /// </summary>
    private void EnemyAnim()
    {
        _time += Time.deltaTime * animSpeed;
        if (_time >= 1f)
        {
            ////Debug.Log("anim");
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
    }

    /// <summary>
    /// Class that allows an enemy to move right and left
    /// </summary>
    public virtual void EnemyMove()
    {

        transform.Translate(new Vector3(_velocity * _dir, 0) * Time.deltaTime);

        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left * _dir), Color.yellow);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.right), out hit, 0.5f) ||
            Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.left), out hit, 0.5f))
        {
            if (hit.transform.gameObject.tag != "Player")
                _dir *= -1;
        }
        if (_dir > 0)
        {
            _renderer.flipX = true;
        }
        else {
            _renderer.flipX = false;
        }
    }

    /// <summary>
    /// Enemy dead changing its sprite and destroying it
    /// </summary>
    public virtual void Die()
    {
        Destroy(this.GetComponent<BoxCollider>());
        Destroy(this.GetComponent<Rigidbody>());
        _dead = true;
        _renderer.sprite = animDead;
        Destroy(gameObject, 1.0f);
    }

    /// <summary>
    /// Check collision with the player
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {           
            _collider.isTrigger = true;
            _rigidbody.useGravity = false;
        }

    }

    /// <summary>
    /// Check collision (trigger mode) with the player
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {         
            _collider.isTrigger = false;
            _rigidbody.useGravity = true;
        }
    }

}
