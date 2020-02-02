using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private bool _onGround = false;
    private bool _maxJump = false;
    private Rigidbody _rigidBody;
    private bool _directionY = false;
    private bool stoppedJumping = false;


    public float jumpTime;


    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = this.transform.parent.GetComponentInParent<Rigidbody>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(_directionY);
        JumpMove();
    }

    private void JumpMove()
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
    }
}
