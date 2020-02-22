using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    private float _time;
    private int _currentAnim = 0; 
    private Vector3 _startPosition;
    private int _dir = -1;

    protected SpriteRenderer _renderer;
    protected bool _dead = false;

    public Sprite[] anim;
    public Sprite animDead;
    public float animSpeed = 4;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_dead)
            EnemyAnim();    
    }


    private void FixedUpdate()
    {
        // si esta dentro del rango de camara
        EnemyMove();
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

    public void EnemyMove()
    {
        if (!_dead)
        {
            transform.Translate(new Vector3(1.0f * _dir, 0) * Time.deltaTime);

            RaycastHit hit;

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left * _dir), Color.yellow);

            if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.right), out hit, 0.5f) ||
                Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.left), out hit, 0.5f))
            {
                if (hit.transform.gameObject.tag != "Player")
                    _dir *= -1;
            }
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
