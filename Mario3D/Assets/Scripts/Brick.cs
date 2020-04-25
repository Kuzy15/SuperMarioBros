using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{

    private Vector2 _startPosition;
    private bool _goDown = false;
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

    public void StartMoveBrick()
    {
        ActivateBounce();
        StartCoroutine("MoveBrickCouroutine");
    }

    IEnumerator MoveBrickCouroutine()
    {
        yield return new WaitForSeconds(1.0f);
        MoveBrick();   
    }


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

    private void ActivateBounce()
    {
        if(!_active)
            _active = true;
    }
}
