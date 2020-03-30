using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{

    public float speed = 1;
    public float shift = 2;
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
    private CapsuleCollider _collider;
    private float _time = 0.5f;
    public float _jumpTime = 0.5f;
    private float _force = 6f; // 3.5f for the little Mario, 6.5f for the big one
    private bool _onGround = true;// false;
    private bool _stoppedJumping = false;
    private bool _isGrowing = false;
    private float velocity = 0;
    private bool isJumping = false;
    private bool invulnerable = false;

    // Start is called before the first frame update
    void Start()
    {
        _collider = this.GetComponentInChildren<CapsuleCollider>();
        _marioSprite = this.GetComponentInChildren<SpriteRenderer>();
        _isBig = false;
        ChangeCollider();
        SetAnim(0, _isBig);

        //_collider.size = _currentAnim[0].bounds.size;
        //_collider.center = _currentAnim[0].bounds.center;
        _rigidBody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -9.8f * _rigidBody.mass, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (!GameCamera.Instance.GetLooking())
        {

            _directionX = Input.GetAxis("Horizontal");
            _directionY = Input.GetAxis("Vertical");

            if (_directionX == 0 && _directionY == 0)
            {
                GameCamera.Instance.SetCanReset();
            }
            else
            {
                GameCamera.Instance.InactiveCanReset();
            }


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





            CheckCollisons(Vector3.up);
            CheckCollisons(Vector3.down);
            CheckCollisons(Vector3.right);
            CheckCollisons(Vector3.left);






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

    }

    private void FixedUpdate()
    {

        JumpMove();

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


    private void CheckCollisons(Vector3 dir)
    {
        RaycastHit hit;
        if (dir == Vector3.down)
        {
            if (Physics.Raycast(transform.position + new Vector3(-_collider.radius / 2 - 0.05f, _collider.height / 2, 0), dir, out hit, (_collider.height / 2) + 0.1f) ||
                Physics.Raycast(transform.position + new Vector3(_collider.radius / 2 + 0.05f, _collider.height / 2, 0), dir, out hit, (_collider.height / 2) + 0.1f))
            {
                // Pintar rayo
                Debug.DrawRay(transform.position + new Vector3(-_collider.radius / 2 - 0.05f, _collider.height / 2, 0), dir * (_collider.height / 2) - new Vector3(0, (-dir.y) * 0.1f, 0), Color.yellow);
                Debug.DrawRay(transform.position + new Vector3(_collider.radius / 2 + 0.05f, _collider.height / 2, 0), dir * (_collider.height / 2) - new Vector3(0, (-dir.y) * 0.1f, 0), Color.yellow);
                //----------------------------------


                if (hit.transform.tag == "Solid")
                {
                    ////Debug.Log("SUELO");
                    _onGround = true;
                    //_jumpTime = _time;

                    if (isJumping)
                    {
                        //Debug.Log("change anim to idle");
                        SetAnim(0, IsBigMario());
                        isJumping = false;
                    }
                }
                else if (hit.transform.tag == "Enemy")
                {
                    //Debug.Log("enemy hit");
                    hit.transform.gameObject.GetComponent<Enemy>().Die();
                    _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _force);
                }


            }
            else
            {
            }
        }
        else if(dir == Vector3.up)
        {
            if (Physics.Raycast(transform.position + new Vector3(0, _collider.height / 2, 0), dir, out hit, (_collider.height / 2) + 0.1f))
            {
                Debug.DrawRay(transform.position + new Vector3(0, _collider.height / 2, 0), dir * (_collider.height / 2) - new Vector3(0, (-dir.y) * 0.1f, 0), Color.yellow);

                if (hit.transform.gameObject.GetComponent<MagicBlock>())
                {
                    hit.transform.gameObject.GetComponent<MagicBlock>().ActivateBlock();
                }

                //Solo cuando es pequeño
                if (hit.transform.gameObject.GetComponent<Brick>())
                {
                    if (_isBig)
                    {
                        // Anim destroy block
                        Destroy(hit.transform.gameObject);
                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<Brick>().StartMoveBrick();
                    }
                }
            }
        }
        else
        {

            if (Physics.Raycast(transform.position + new Vector3(0, _collider.height / 2, 0), dir, out hit, (_collider.height / 2) - 0.2f))
            {
                Debug.DrawRay(transform.position + new Vector3(0, _collider.height / 2, 0), dir * (_collider.height / 2) - new Vector3(0, (-dir.y) * 0.1f, 0), Color.yellow);

                if (hit.transform.gameObject.GetComponent<MagicBlock>())
                {
                    hit.transform.gameObject.GetComponent<MagicBlock>().ActivateBlock();
                }
                if (hit.transform.gameObject.GetComponent<Enemy>())
                {
                    if (_isBig)
                    {
                        invulnerable = true;
                        _isBig = false;
                        ChangeCollider();
                        Invoke("SetInvulnerable", 1.5f);                      
                    }
                    else
                    {
                        if (!invulnerable)
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }
            }
        }

    }

    private void SetInvulnerable()
    {
        invulnerable = false;
    }


    private void JumpMove()
    {
        if (Input.GetButtonDown("Vertical"))
        {

            //and you are on the ground...
            if (_onGround && _jumpTime > 0)
            {
                ////Debug.Log("to jump");
                isJumping = true;
                _onGround = false;
                SetAnim(2, IsBigMario());
                //jump!
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _force);
                _stoppedJumping = false;
            }

        }

        //if you keep holding down the mouse button...
        if ((Input.GetButton("Vertical")) && !_stoppedJumping)
        {
            //and your counter hasn't reached zero...
            if (_jumpTime > 0)
            {
                //keep jumping!
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _force);
                _jumpTime -= Time.deltaTime;
            }
            else
            {
                _stoppedJumping = true;
            }
        }


        //if you stop holding down the mouse button...
        if (Input.GetButtonUp("Vertical"))
        {
            //stop jumping and set your counter to zero.  The timer will reset once we touch the ground again in the update function.
            _jumpTime = _time;
            _stoppedJumping = true;
        }
    }


    private void ChangeCollider()
    {
        if (_isBig)
        {
            _collider.height = 2;
            _collider.center = new Vector3(0, 1, 0);
            _force = 7.5f;
        }
        else
        {
            _collider.height = 1;
            _collider.center = new Vector3(0, 0.5f, 0);
            _force = 6f;
        }
    }

    private void Invulnerable()
    {
        _collider.enabled = true;
        _rigidBody.useGravity = true;
        //Debug.Log("MARIO COLLIDER --------------- ");
    }

    private void CheckDead()
    {

        //Debug.Log("MARIO ENEMY --------------- ");


        // If Mario is big and collisions with an enemy
        if (_isBig)
        {
            /* _collider.enabled = false;
             _rigidBody.useGravity = false;
             Invoke("Invulnerable", 1.0f);*/
            //Debug.Log("MARIO LITTLE --------------- ");
            // Make Mario little
            ChangeCollider();
            _isBig = false;
        }
        else
        {
            // Mario dies
            //Debug.Log("MARIO DEAD --------------- ");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Mushroom>())
        {

            if (!_isBig)
            {
                _isGrowing = true;
                GrowUp();
            }
            Destroy(collision.collider.gameObject);
        }
        /*if (collision.collider.GetComponent<Enemy>())
        {
            CheckDead();
        }*/
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.GetComponent<Enemy>())
        {
            CheckDead();
        }
    }

    public Vector3 GetMarioPosition()
    {
        return transform.position;
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
