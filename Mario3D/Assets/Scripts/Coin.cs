using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coin : MonoBehaviour
{
    public Sprite[] anim;
    public Sprite[] animInstantiated;
    public float animSpeed = 0;
    public float animInstantiatedSpeed = 10;
    public bool _canMove = true;
   

    private float _time;
    private int _currentAnim;
    private SpriteRenderer _renderer;
    private Vector3 _startPosition;
    private bool _down = false;

    private bool _isInstantiated = false;
    private Sprite[] _animToUse;
    private float _speedToUse;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _startPosition = this.transform.position;
        if (_isInstantiated && animInstantiated.Length > 0)
        {
            _animToUse = animInstantiated;
            _speedToUse = animInstantiatedSpeed;
        }
        else
        {
            _animToUse = anim;
            _speedToUse = animSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CoinAnim();
        if (_canMove)
        {
            CoinMove();
        }
    }

    private void CoinAnim()
    {
        _time += Time.deltaTime * _speedToUse;
        if (_time >= 1f)
        {
            _currentAnim++;
            _time = 0;
        }

        if (_currentAnim < anim.Length)
        {
            _renderer.sprite = _animToUse[_currentAnim];
        }
        else
        {
            _currentAnim = 0;
        }
    }

    private void CoinMove()
    {
        float step = 12f * Time.deltaTime; // calculate distance to move
        if (!_down)
        {
            transform.position = Vector3.MoveTowards(transform.position, _startPosition + new Vector3(0, 2.5f), step);
            if (Vector3.Distance(transform.position, _startPosition + new Vector3(0, 2.5f)) < 0.001f)
            {
                // Swap the position of the cylinder.
                _down = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _startPosition, step);
            if (Vector3.Distance(transform.position, _startPosition) < 0.001f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void SetMove(bool canMove)
    {
        _canMove = canMove;
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (!_canMove)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetInstantiated() {
        _isInstantiated = true;
    }

}
