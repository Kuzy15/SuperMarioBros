using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that represents a brick that bounces (not destroy) on collision
/// </summary>
public class Brick : MonoBehaviour
{
    //start position of the brick
    private Vector2 _startPosition;
    //bool to check if a brick can bounce
    private bool _goDown = false;
    //bool to activate bouncing
    private bool _active = false;


    // Start is called before the first frame update
    void Start()
    {
        _startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveBrick();
    }

    /// <summary>
    /// Function that allows to a brick to make a bouncing move
    /// </summary>
    public void StartMoveBrick()
    {
        ActivateBounce();
        StartCoroutine("MoveBrickCouroutine");
    }

    /// <summary>
    /// Coroutine function that starts bounce moving for a second
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveBrickCouroutine()
    {
        yield return new WaitForSeconds(1.0f);
        MoveBrick();   
    }

    /// <summary>
    /// Function that calculates the distance from an start position to an end point. On equal distance, the brick is allowed to go down.
    /// Represents the whole bouncing movement.
    /// </summary>
    private void MoveBrick()
    {
        if (_active)
        {
            if ((Vector2)transform.position == _startPosition + new Vector2(0, 0.3f) && !_goDown)
            {
                _goDown = true;
            }

            if (!_goDown)
            {
                transform.position = Vector2.MoveTowards(transform.position, _startPosition + new Vector2(0, 0.3f), 3 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, _startPosition, 3 * Time.deltaTime);
                if (Vector2.Distance(transform.position, _startPosition) < 0.001f)
                {
                    _active = false;
                    _goDown = false;
                }
            }
        }
    }

    /// <summary>
    /// Activate bouncing flag (_active)
    /// </summary>
    private void ActivateBounce()
    {
        if(!_active)
            _active = true;
    }
}
