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
    public Sprite[] toBigMario;
    public Sprite[] toSmallMario;


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

    private bool _isGrowing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _collider = this.GetComponentInChildren<BoxCollider>();
        _marioSprite = this.GetComponentInChildren<SpriteRenderer>();
        _isBig = false;
        ChangeCollider();
        SetAnim(0, _isBig);

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

        if (!_isGrowing)
        {
            if (_directionX < 0)
            {
                _marioSprite.flipX = true;
                SetAnim(1, _isBig);
            }
            else if (_directionX > 0)
            {
                _marioSprite.flipX = false;
                SetAnim(1, _isBig);
            }
            else
            {
                SetAnim(0, _isBig);
            }
            if (_directionY > 0)
            {
                //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.05f, this.transform.position.z);
                SetAnim(2, _isBig);
            }
        }
        else
        {
            SetAnim(3, !_isBig);
        }
        velocity = Mathf.Lerp(velocity, _directionX, shift * Time.deltaTime);

        if (velocity < 0.000003f && velocity > -0.000003f)
        {
            velocity = 0;
        }

        Vector2 pos = this.transform.position;
        pos.x += (speed * velocity * Time.deltaTime);
        transform.position = pos;

        if (!_isGrowing)
        {
            _animTime += Time.deltaTime * (Mathf.Abs(velocity)) * (speed * 2);
        }
        else
        {
            _animTime += Time.deltaTime * 15;
        }

        if ((int)_animTime >= 1)
        {
            _animTime = 0;
            _currentSprite++;
        }
        if (_currentAnim != null)
        {
            if (_currentSprite >= _currentAnim.Length)
            {
                _currentSprite = 0;
                if (_isGrowing)
                {
                    _isGrowing = false;
                    _currentSprite = _currentAnim.Length - 1;
                }
            }
            _marioSprite.sprite = _currentAnim[_currentSprite];
        }


    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            _isGrowing = true;
            GrowUp();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isGrowing = true;
            GrowDown();
        }

    }



    public void SetAnim(int state, bool big)
    {
        if (_animState != state)
        {
            _currentSprite = 0;
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
                case 3:
                    if (big)
                        _currentAnim = toSmallMario;
                    else
                        _currentAnim = toBigMario;
                    break;
            }
            _animState = state;
            //_collider.center = _currentAnim[0].bounds.center;
        }
    }

    private void ChangeCollider()
    {
        if (_isBig)
        {
            _collider.size = new Vector3(1, 2, 1);
            _collider.center = new Vector3(0, 1, 0);
        }
        else
        {
            _collider.size = new Vector3(1, 1, 1);
            _collider.center = new Vector3(0, 0.5f, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<MagicBlock>())
        {
            collision.collider.GetComponent<MagicBlock>().ActivateBlock();
        }

        //Solo cuando es pequeño
        if (collision.collider.GetComponent<Brick>())
        {
            collision.collider.GetComponent<Brick>().ActivateBounce();
        }
    }

    public Vector3 GetMarioPosition()
    {
        return this.transform.position;
    }

    public bool IsBigMario()
    {
        return _isBig;
    }

    public void GrowUp()
    {
        _isBig = true;
        ChangeCollider();
    }

    public void GrowDown()
    {
        _isBig = false;
        ChangeCollider();
    }
}
