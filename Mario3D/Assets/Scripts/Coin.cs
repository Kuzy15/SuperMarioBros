using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coin : MonoBehaviour
{
    private float _time;
    private int _currentAnim;
    private SpriteRenderer _renderer;
    private Vector3 startPosition;

    public Sprite[] anim;
    public float animSpeed = 0;
    bool down = false;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CoinMove();
        Debug.Log(startPosition + "    " + transform.position);
        float step = 12f * Time.deltaTime; // calculate distance to move
        if (!down)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition + new Vector3(0, 2.5f), step);
            if (Vector3.Distance(transform.position, startPosition + new Vector3(0, 2.5f)) < 0.001f)
            {
                // Swap the position of the cylinder.
                down = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, step);
            if (Vector3.Distance(transform.position, startPosition) < 0.001f)
            {
                // Swap the position of the cylinder.
                AddPoint();
                Destroy(this.gameObject);
            }
        }
    }

    private void CoinMove()
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

    public void AddPoint()
    {

    }
}
