using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    private float _time;
    private int _currentAnim = 0; 

    protected Vector3 _startPosition;
    protected SpriteRenderer _renderer;
    protected BoxCollider _collider;
    protected int _dir = -1;
    protected float _velocity = 1.0f;
    protected bool _dead = false;
    protected bool _canMove = true;

    public Sprite[] anim;
    public Sprite animDead;
    public float animSpeed = 4;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _collider = this.GetComponent<BoxCollider>();
        _startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_dead)
        {
            EnemyAnim();
        }
    }


    private void FixedUpdate()
    {
        // si esta dentro del rango de camara
        if (_canMove)
        {
            EnemyMove();
        }
    }

    private void EnemyAnim()
    {
        _time += Time.deltaTime * animSpeed;
        if (_time >= 1f)
        {
            Debug.Log("anim");
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
    }

    public void AddPoint()
    {

    }

    public virtual void Die()
    {
        _dead = true;
        _renderer.sprite = animDead;
        Destroy(gameObject, 1.0f);
    }

}
