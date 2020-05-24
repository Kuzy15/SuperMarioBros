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
    public Sprite[] swimAnim;
    public Sprite[] smallSwimAnim;
    public UnityEngine.UI.Image blackImage;


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
    private float _velocity = 0;
    private bool _isJumping = false;
    private bool _invulnerable = false;
    private bool _isGroundedJ;
    private bool _goingDown = false;
    private bool _goingUp = false;
    private bool _goingRight = false;
    private bool _downAnim = false;
    private bool _playerInCreeper = false;
    private bool _canClimb = false;
    private bool _cameraSaved = false;
    private bool _goingDowOfCreeper = false;
    private float _lastYCamera;
    private Shuttle _shuttle;
    private bool _isUnderground = false;
    private bool _isInWater = false;


    private Vector3 _startPosition;

    private RaycastHit auxHit;
    // Start is called before the first frame update
    void Start()
    {
        _collider = this.GetComponentInChildren<CapsuleCollider>();
        _marioSprite = this.GetComponentInChildren<SpriteRenderer>();
        _isBig = false;
        ChangeCollider();
        SetAnim(0, _isBig);
        blackImage.gameObject.SetActive(false);

        //_collider.size = _currentAnim[0].bounds.size;
        //_collider.center = _currentAnim[0].bounds.center;
        _rigidBody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -9.8f * _rigidBody.mass, 0);
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 25f, this.gameObject.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!LoadScene.Instance.GetStart())
        {
            if (_goingDown)
            {
                EnterMove();
            }
            if (_goingUp)
            {
                ExitMove();
            }
            if (_goingRight)
            {
                ExitSecretZoneMove();
            }
            _isGroundedJ = (_rigidBody.velocity.y == 0);

            if (!_playerInCreeper && !GameCamera.Instance.GetLooking())
            {
                _directionX = Input.GetAxis("Horizontal");
                _directionY = Input.GetAxis("Vertical");

                if (!_isGrowing)
                {
                    if (_isInWater)
                    {
                        SetAnim(4, _isBig);
                    }
                    else
                    {
                        SetAnim(1, _isBig);
                    }
                    if (_directionX < 0)
                    {
                        _marioSprite.flipX = true;
                    }
                    else if (_directionX > 0)
                    {
                        _marioSprite.flipX = false;
                    }
                    else
                    {
                        if (!_isInWater)
                            SetAnim(0, _isBig);
                    }
                }
                else
                {
                    SetAnim(3, !_isBig);
                }

                _velocity = Mathf.Lerp(_velocity, _directionX, shift * Time.deltaTime);

                if (_velocity < 0.000003f && _velocity > -0.000003f)
                {
                    _velocity = 0;
                }

                Vector2 pos = this.transform.position;
                pos.x += (speed * _velocity * Time.deltaTime);

                CheckCollisons(Vector3.up);
                if (!_goingDown)
                {
                    CheckCollisons(Vector3.down);
                }
                CheckCollisons(Vector3.right);
                CheckCollisons(Vector3.left);
                CheckOnShuttle();
                transform.position = pos;

                if (!_isGrowing)
                {
                    _animTime += Time.deltaTime * (Mathf.Abs(_velocity)) * (speed * 2);
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
    }

    private void FixedUpdate()
    {
        if (!GameCamera.Instance.GetLooking() && !LoadScene.Instance.GetStart())
        {
            CheckSecretZone();
            if (!_isInWater)
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

                if (_shuttle != null)
                {

                }
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    _rigidBody.AddForce(new Vector2(0, 6f * Time.deltaTime), ForceMode.Impulse);
                }
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    _rigidBody.AddForce(new Vector2(0, -6f * Time.deltaTime), ForceMode.Impulse);
                }
                _rigidBody.AddForce(new Vector2(0, 8f * Time.deltaTime), ForceMode.Impulse);
                Debug.Log("IN WATER");
            }
        }
    }

    private void CheckSecretZone()
    {
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            CheckEnterSecretZone();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            CheckExitSecretZone();
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
                case 4:
                    if (big)
                        _currentAnim = swimAnim;
                    else
                        _currentAnim = smallSwimAnim;
                    break;
            }
            _animState = state;
        }
    }

    private void CheckEnterSecretZone()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(-_collider.radius / 2 - 0.05f, _collider.height / 2, 0), Vector3.down, out hit, (_collider.height / 2) + 0.1f) ||
                Physics.Raycast(transform.position + new Vector3(_collider.radius / 2 + 0.05f, _collider.height / 2, 0), Vector3.down, out hit, (_collider.height / 2) + 0.1f))
        {
            if (hit.transform.gameObject.GetComponent<EnterSecretZone>())
            {
                auxHit = hit;
                Debug.Log("ENTERRRRRR");
                this.GetComponentInChildren<SpriteRenderer>().sortingOrder = -2;
                //EnterMove(hit);
                _startPosition = this.transform.position;
                _goingDown = true;
            }
        }
    }

    private void CheckExitSecretZone()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, _collider.height, 0), Vector3.right, out hit, 1f))
        {
            if (hit.transform.gameObject.GetComponent<ExitSecretZone>())
            {
                auxHit = hit;
                Debug.Log("EXITTTTT");
                //AnimatePlayer();
                _startPosition = this.transform.position;
                _goingRight = true;
            }
        }
    }

    public IEnumerator SecretZoneCoroutine()
    {

        //yield return new WaitForSeconds(1f);
        blackImage.gameObject.SetActive(true);
        this.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
        yield return new WaitForSeconds(1.5f);
        //_rigidBody.useGravity = false;
        auxHit.transform.gameObject.GetComponent<EnterSecretZone>().GoToSecretZone(this);
        if (auxHit.transform.gameObject.GetComponent<EnterSecretZone>().GetIfUnderground())
        {
            GameCamera.Instance.GoToBlackScreen();
            GameCamera.Instance.SetCameraY(-46.5f);
            _isUnderground = true;
            _isInWater = false;
        }
        else
        {
            GameCamera.Instance.SetCameraY(-31.5f);
            _isUnderground = false;
            //_rigidBody.useGravity = false;
            _isInWater = true;
            Physics.gravity = new Vector3(0, -9.8f * _rigidBody.mass, 0);
        }
        //GameCamera.Instance.ResetCamera();

        blackImage.gameObject.SetActive(false);
        GameCamera.Instance.SetCameraX(this.transform.position.x + 10f);
        yield return new WaitForSeconds(0.5f);
        _rigidBody.useGravity = true;
    }

    private IEnumerator ExitSecretZone()
    {
        blackImage.gameObject.SetActive(true);
        GameCamera.Instance.SetCameraY(-16.5f);
        //GameCamera.Instance.SetCameraX(this.transform.position.x - 6);
        yield return new WaitForSeconds(1f);
        //_goingDown = true;
        //GrowUp();
        blackImage.gameObject.SetActive(false);
        ExitSecretZone aux = auxHit.transform.gameObject.GetComponent<ExitSecretZone>();
        if (_isUnderground)
        {
            GameCamera.Instance.GoToBlueScreen();
        }
        //ChangeCollider();
        aux.GoToSecretZone(this);
        _startPosition = aux.GetSecretZonePosition();
        _goingRight = false;
        _goingUp = true;
        _isInWater = false;
        _rigidBody.mass = 3f;
        Physics.gravity = new Vector3(0, -9.8f * _rigidBody.mass, 0);
        // ExitMove(hit);
        //this.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
    }
    public void ExitSecretZoneMove()
    {
        this.GetComponentInChildren<SpriteRenderer>().sortingOrder = -2;
        _rigidBody.useGravity = false;
        _rigidBody.detectCollisions = false;
        _rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        float step = 2f * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, _startPosition + new Vector3(2, 0f), step);
        if (Vector3.Distance(transform.position, _startPosition + new Vector3(2, 0f)) < 0.001f)
        {
            // Swap the position of the cylinder.
            _rigidBody.useGravity = true;
            _rigidBody.detectCollisions = true;
            _rigidBody.constraints = RigidbodyConstraints.None;
            _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            _goingRight = false;
            this.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
            StartCoroutine(ExitSecretZone());
            //StartCoroutine(ExitSecretZone());
            //_downAnim = true;
            //EnterMove();
        }
    }
    public void ExitMove()
    {
        this.GetComponentInChildren<SpriteRenderer>().sortingOrder = -2;
        _rigidBody.useGravity = false;
        _rigidBody.detectCollisions = false;
        _rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        float step = 2f * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, _startPosition + new Vector3(0, 0.5f), step);
        if (Vector3.Distance(transform.position, _startPosition + new Vector3(0, 0.5f)) < 0.001f)
        {
            // Swap the position of the cylinder.
            _rigidBody.useGravity = true;
            _rigidBody.detectCollisions = true;
            _rigidBody.constraints = RigidbodyConstraints.None;
            _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            _goingUp = false;
            this.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
            //StartCoroutine(ExitSecretZone());
            //_downAnim = true;
            //EnterMove();
        }
    }

    public void EnterMove()
    {
        _rigidBody.useGravity = false;
        _rigidBody.detectCollisions = false;
        _rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        float step = 2f * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, _startPosition - new Vector3(0, 2f), step);
        if (Vector3.Distance(transform.position, _startPosition - new Vector3(0, 2f)) < 0.001f)
        {
            // Swap the position of the cylinder.
            _rigidBody.useGravity = true;
            _rigidBody.detectCollisions = true;
            _rigidBody.constraints = RigidbodyConstraints.None;
            _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            _goingDown = false;
            StartCoroutine(SecretZoneCoroutine());
            //_downAnim = true;
            //EnterMove();
        }
    }

    public void AnimatePlayer()
    {
        MoveMario();
        //this.GetComponentInChildren<Transform>().Translate(Vector3.down * Time.deltaTime);
    }

    private void CheckCollisons(Vector3 dir)
    {
        RaycastHit hit;
        if (dir == Vector3.down)
        {
            Debug.Log("COLLISION DOWN");
            if (Physics.Raycast(transform.position + new Vector3(-_collider.radius / 2 - 0.05f, _collider.height / 2, 0), dir, out hit, (_collider.height / 2) + 0.1f) ||
                Physics.Raycast(transform.position + new Vector3(_collider.radius / 2 + 0.05f, _collider.height / 2, 0), dir, out hit, (_collider.height / 2) + 0.1f))
            {

                // Pintar rayo
                // Debug.DrawRay(transform.position + new Vector3(-_collider.radius / 2 - 0.05f, _collider.height / 2, 0), dir * (_collider.height / 2) - new Vector3(0, (-dir.y) * 0.1f, 0), Color.yellow);
                // Debug.DrawRay(transform.position + new Vector3(_collider.radius / 2 + 0.05f, _collider.height / 2, 0), dir * (_collider.height / 2) - new Vector3(0, (-dir.y) * 0.1f, 0), Color.yellow);
                //----------------------------------


                if (hit.transform.tag == "Solid")
                {
                    _onGround = true;

                    if (!_isInWater && _isJumping)
                    {
                        SetAnim(0, IsBigMario());
                        _isJumping = false;
                    }
                }
                else if (hit.transform.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<Enemy>().Die();
                    _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _force);
                }

            }
        }
        else if (dir == Vector3.up)
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
                        _invulnerable = true;
                        _isBig = false;
                        ChangeCollider();
                        Invoke("SetInvulnerable", 1.5f);
                    }
                    else
                    {
                        if (!_invulnerable)
                            GameManager.GM.ChangeScene(GameManager.SceneFlow.CURRENT);
                    }
                }
            }
        }

    }

    private void SetInvulnerable()
    {
        _invulnerable = false;
    }


    private void JumpMove()
    {
        if (!GameCamera.Instance.GetLooking() && !_canClimb)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("JUMPO");
                //and you are on the ground...
                if (!_isInWater && _onGround && _jumpTime > 0)
                {
                    ////Debug.Log("to jump");
                    _isJumping = true;
                    _onGround = false;
                    SetAnim(2, IsBigMario());
                    //jump!
                    _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _force);
                    _stoppedJumping = false;
                }

            }

            //if you keep holding down the mouse button...
            if (((Input.GetKey(KeyCode.UpArrow)) || (Input.GetKey(KeyCode.W))) && !_stoppedJumping)
            {
                CheckBreakableBrick();
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
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                //stop jumping and set your counter to zero.  The timer will reset once we touch the ground again in the update function.
                _jumpTime = _time;
                //_stoppedJumping = true;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                /*_goingDowOfCreeper = true;
                if (!_cameraSaved)
                {
                    _cameraSaved = true;
                    _lastYCamera = GameCamera.Instance.GetCameraY();
                    GameCamera.Instance.CanFollowInY(true);
                }
                _playerInCreeper = true;*/
                _playerInCreeper = true;
                this.gameObject.transform.Translate(Vector3.up * Time.deltaTime * 2f);
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {


                _cameraSaved = false;
                this.gameObject.transform.Translate(-Vector3.up * Time.deltaTime * 2f);
                //_rigidBody.useGravity = true;
                //_playerInCreeper = false;
                _canClimb = false;
                /*if(_rigidBody.velocity.y == 0)
                {
                    Debug.Log("IM OUT OF CREEPER");
                    _playerInCreeper = false;
                    _goingDowOfCreeper = true;
                    _canClimb = false;
                    GameCamera.Instance.SetCameraY(_lastYCamera);
                }*/
            }
            /*if(_onGround && !_playerInCreeper)
            {
                    _rigidBody.useGravity = true;
                    _playerInCreeper = false;
                    _canClimb = false;
            }*/
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
    }

    private void CheckDead()
    {
        // If Mario is big and collisions with an enemy
        if (_isBig)
        {
            ChangeCollider();
            _isBig = false;
        }
        else
        {
            GameManager.GM.ChangeScene(GameManager.SceneFlow.CURRENT);
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
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.GetComponent<Enemy>())
        {
            CheckDead();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ExitWater"))
        {

        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (!_playerInCreeper && !_isJumping && other.GetComponent<Creeper>())
        {
            Debug.Log("IM IN CREEPER");
            _rigidBody.useGravity = false;
            _canClimb = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        /*if (other.GetComponent<Creeper>())
        {
            Debug.Log("IM IN CREEPER");
        }*/
        Debug.Log("IM NOT IN CREEPER");
        _playerInCreeper = false;
        //_canClimb = false;


        //GameCamera.Instance.CanFollowInY(false);
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

    public void MoveMario(bool up = true)
    {
        int value = 1;
        if (!up)
        {
            value = -1;
        }
        this.GetComponentInChildren<CapsuleCollider>().enabled = false;
        this.GetComponentInChildren<SpriteRenderer>().sortingOrder = -2;
        _goingDown = true;
        transform.Translate(value * Vector3.down * Time.deltaTime / 4f);
        //GetComponentInParent<Rigidbody>().AddForce(Vector3.up * 2f);
        //this.tr
        /*if (_downAnim)
        {
            this.GetComponent<Transform>().localPosition = new Vector3(this.GetComponent<Transform>().localPosition.x, this.GetComponent<Transform>().localPosition.y,
                   this.GetComponent<Transform>().localPosition.z - 2);
        }*/
    }

    public void CheckBreakableBrick()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, _collider.height / 2, 0), Vector3.up, out hit, (_collider.height / 2) + 0.2f))
        {
            if (hit.transform.gameObject.GetComponent<BreakableBrick>())
            {
                hit.transform.gameObject.GetComponent<BreakableBrick>().DestroyBrick();
            }
        }
    }

    private void CheckOnShuttle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(-_collider.radius / 2 - 0.05f, _collider.height / 2, 0), Vector3.down, out hit, (_collider.height / 2) + 0.1f) ||
                Physics.Raycast(transform.position + new Vector3(_collider.radius / 2 + 0.05f, _collider.height / 2, 0), Vector3.down, out hit, (_collider.height / 2) + 0.1f))
        {
            if (hit.transform.gameObject.GetComponent<Shuttle>())
            {
                _shuttle = hit.transform.gameObject.GetComponent<Shuttle>();
                _shuttle.StartShuttle();
                ShuttlePlayer();
            }
        }
    }

    public void ShuttlePlayer()
    {
        _rigidBody.AddForce(Vector3.up * 90.5f - _rigidBody.velocity, ForceMode.Impulse);
    }
}
