using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private bool _onGround = true;// false;
    private Rigidbody _rigidBody;
    private bool _directionY = false;
    private bool stoppedJumping = false;

    public float time = 0;
    public float jumpTime = 0;
    public float force = 0;

    private Player _player;


    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = this.transform.parent.GetComponentInParent<Rigidbody>();
        _player = this.transform.parent.GetComponentInParent<Player>();
    }

    private void Update()
    {
        _directionY = Input.GetButtonDown("Vertical");
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(_directionY);
        JumpMove();
    }

    private void JumpMove()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            //and you are on the ground...
            if (_onGround)
            {
                _player.SetAnim(2, _player.IsBigMario());
                //jump!
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, force);
                stoppedJumping = false;
            }

        }

        //if you keep holding down the mouse button...
        if ((Input.GetButton("Vertical")) && !stoppedJumping)
        {
            //and your counter hasn't reached zero...
            if (jumpTime > 0)
            {
                //keep jumping!
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, force);
                jumpTime -= Time.deltaTime;
            }
            else
            {
                stoppedJumping = true;
            }
        }


        //if you stop holding down the mouse button...
        if (Input.GetButtonUp("Vertical"))
        {
            //stop jumping and set your counter to zero.  The timer will reset once we touch the ground again in the update function.
            jumpTime = 0;
            stoppedJumping = true;
        }
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Solid")
        {
            _onGround = true;
            jumpTime = time;
        }
    }
    void OnTriggerStay(Collider col)
    {
        if (!_onGround && col.gameObject.tag == "Solid")
        {
            _onGround = true;
            jumpTime = time;
        }
    }












    /*private void JumpMove()
    {
        Debug.Log(jumpTime);

        if(Input.GetButton("Vertical") && !stoppedJumping)
        {
            if(jumpTime <= 0.5f)
            {
                jumpTime += Time.deltaTime;
                //jumpTime = 1;
            }
            else
            {
                jumpTime = 0.5f;
                _maxJump = true;
            }
            if (!_maxJump)
            {
                _rigidBody.AddForce(new Vector2(_rigidBody.velocity.x, jumpTime*3.5f), ForceMode.Impulse);
            }
            _onGround = false;
        }

        if (Input.GetButtonUp("Vertical"))
        {
            stoppedJumping = true; 
            jumpTime = 0;
        }
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Solid" && !_onGround)
        {
            _onGround = true;
            stoppedJumping = false;
            _maxJump = false;
        }
    }*/
}
