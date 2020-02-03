using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 1;
    public float shift = 2;
    public float jumpSpeed = 2;
    public float velocity = 0;
    public float gravity = 1;

    public Sprite[] smallWalk;
    public Sprite[] smallIdle;
    public Sprite[] bigWalk;
    public Sprite[] bigIdle;
    public Sprite[] jump;
    public Sprite[] smallJump;

    private SpriteRenderer _marioSprite;
    private Sprite[] _currentAnim;
    private float _directionX = 0;
    private float _directionY = 0;
    private bool _isBig = true;
    private int _animState = 0;
    private float _animTime = 0;
    private int _currentSprite = 0;
    private Rigidbody _rigidBody;
    private BoxCollider _collider;
    private bool _mario;
    
    // Start is called before the first frame update
    void Start()
    {
        _mario = true;
        _collider = this.GetComponentInChildren<BoxCollider>();
        _marioSprite = this.GetComponentInChildren<SpriteRenderer>();
        SetAnim(0, _mario);
        _currentAnim = bigIdle;
        //_collider.size = _currentAnim[0].bounds.size;
        //_collider.center = _currentAnim[0].bounds.center;
        _rigidBody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, gravity * _rigidBody.mass, 0);
    }

    // Update is called once per frame
    void Update()
    {
        _directionX = Input.GetAxis("Horizontal");
        _directionY = Input.GetAxis("Vertical");

        if (_directionX < 0)
        {
            _marioSprite.flipX = true;
            SetAnim(1, _mario);
        }
        else if(_directionX > 0)
        {
            _marioSprite.flipX = false;
            SetAnim(1, _mario);
        }
        else
        {
            SetAnim(0, _mario);
        }
        if(_directionY > 0)
        {
            //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.05f, this.transform.position.z);
            SetAnim(2, _mario);
        }
        velocity = Mathf.Lerp(velocity, _directionX, shift * Time.deltaTime);

        if(velocity < 0.000003f && velocity > -0.000003f)
        {
            velocity = 0;
        }

        Vector2 pos = this.transform.position;
        pos.x += (speed * velocity * Time.deltaTime);
        transform.position = pos;

        _animTime += Time.deltaTime * (Mathf.Abs(velocity))*(speed*2);

        if ((int)_animTime >= 1)
        {
            _animTime = 0;
            _currentSprite++;
        }
        if (_currentAnim!=null)
        {
            if (_currentSprite >= _currentAnim.Length)
            {
                _currentSprite = 0;
            }
            _marioSprite.sprite = _currentAnim[_currentSprite];
        }
    }

    private void FixedUpdate()
    {
        
    }

   

    public void SetAnim(int state, bool big)
    {
        if (_animState != state)
        {
            switch (state)
            {
                case 0:
                    if (big)
                        _currentAnim = bigIdle;
                    else
                        _currentAnim = smallIdle;
                    break;
                case 1:
                    if (big)
                        _currentAnim = bigWalk;
                    else
                        _currentAnim = smallWalk;
                    break;
                case 2:
                    if (big)
                        _currentAnim = jump;
                    else
                        _currentAnim = smallJump;
                    break;
            }
            _animState = state;
            //_collider.center = _currentAnim[0].bounds.center;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Block>())
        {
            collision.collider.GetComponent<Block>().ActivateBlock();
        }
        if (collision.collider.GetComponent<BouncingBlock>())
        {
            collision.collider.GetComponent<BouncingBlock>().ActivateBounce();
        }
    }

    public Vector3 GetMarioPosition()
    {
        return this.transform.position;
    }
}
