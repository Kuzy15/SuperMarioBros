using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBlock : MonoBehaviour
{
    private bool _goDown = false;
    private Vector2 _startPosition;
    private bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        _startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(active);
        if (active)
        {
            Debug.Log("ENRTRO");
            if ((Vector2)transform.position == _startPosition + new Vector2(0, 0.3f) && !_goDown)
            {
                Debug.Log("AHI");
                _goDown = true;
            }

            if (!_goDown)
            {
                transform.position = Vector2.MoveTowards(transform.position, _startPosition + new Vector2(0, 0.3f), 3 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, _startPosition, 3 * Time.deltaTime);
                if(Vector2.Distance(transform.position, _startPosition) < 0.001f)
                {
                    active = false;
                    _goDown = false;
                }
            }
        }
    }

    public void ActivateBounce()
    {
        active = true;
    }
}
