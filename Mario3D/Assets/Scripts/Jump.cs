using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private bool _onGround = false;
    private bool _maxJump = false;
    private Rigidbody _rigidBody;
    private bool _directionY = false;


    public float jumpForce = 0;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = this.transform.parent.GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        _directionY = Input.GetButton("Vertical");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(jumpForce);
        JumpMove();

        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Solid"))
            {
                Debug.Log("CHSNGE");
                _onGround = true;
            }
            // Apply the force to the rigidbody.
            //_dir *= -1;
        }*/
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down));
    }

    private void JumpMove()
    {
        if (_directionY)
        {
            jumpForce++;
            if (jumpForce >= 15.0f)
            {
                _directionY = false;
                //_maxJump = true;
                jumpForce = 5.0f;
            }
        }

        if (_directionY /*&& !_maxJump*/ && _onGround)
        {
            _rigidBody.AddForce(new Vector2(_rigidBody.velocity.x, jumpForce), ForceMode.Impulse);
            _onGround = false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Solid" && !_onGround)
        {
            _onGround = true;
            //_maxJump = false;
        }
    }
}
