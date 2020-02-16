using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    /*private float _time;
    private int _currentAnim;
    private SpriteRenderer _renderer;*/

    private Vector2 _startPosition;
    private bool _goDown = false;
    private bool active = false;

    /*public Sprite[] anim;
    public float animSpeed = 0;*/

    // Start is called before the first frame update
    void Start()
    {
        //_renderer = this.GetComponent<SpriteRenderer>();
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
        if (active)
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
                    active = false;
                    _goDown = false;
                }
            }
        }
    }

    /*private void DestroyBrick()
    {
        _time += Time.deltaTime * animSpeed;
        if (_time >= 1f)
        {
            _currentAnim++;
            _time = 0;
        }

        if (_currentAnim == anim.Length)
        {
            Destroy(this.gameObject);
        }
    }*/

    private void ActivateBounce()
    {
        if(!active)
            active = true;
    }
}
