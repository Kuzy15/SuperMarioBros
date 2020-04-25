using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coin : MonoBehaviour
{
    public Sprite[] anim;
    public float animSpeed = 0;
   

    private float _time;
    private int _currentAnim;
    private SpriteRenderer _renderer;
    private Vector3 _startPosition;
    private bool _down = false;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CoinAnim();
        CoinMove();
    }

    private void CoinAnim()
    {
        _time += Time.deltaTime * animSpeed;
        if (_time >= 1f)
        {
            _currentAnim++;
            _time = 0;
        }

        if (_currentAnim < anim.Length)
        {
            _renderer.sprite = anim[_currentAnim];
        }
        else
        {
            _currentAnim = 0;
        }
    }

    private void CoinMove()
    {
        float step = 12f * Time.deltaTime; // calculate distance to move
        if (!_down)
        {
            transform.position = Vector3.MoveTowards(transform.position, _startPosition + new Vector3(0, 2.5f), step);
            if (Vector3.Distance(transform.position, _startPosition + new Vector3(0, 2.5f)) < 0.001f)
            {
                // Swap the position of the cylinder.
                _down = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _startPosition, step);
            if (Vector3.Distance(transform.position, _startPosition) < 0.001f)
            {
                Destroy(this.gameObject);
            }
        }
    }

}
