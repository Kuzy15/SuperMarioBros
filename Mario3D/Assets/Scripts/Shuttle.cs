using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuttle : MonoBehaviour
{
    public Sprite[] anim;
    public float animSpeed = 0;

    private float _time;
    private int _currentAnim;
    private SpriteRenderer _renderer;
    private bool _finishedSuttle = true;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_finishedSuttle)
        {
            ShuttleAnim();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _finishedSuttle = false;
        }
    }

    private void ShuttleAnim()
    {
        _time += Time.deltaTime * animSpeed;
        if (!_finishedSuttle && _time >= 1f)
        {
            _currentAnim++;
            _time = 0;
        }

        if (_currentAnim < anim.Length)
        {
            if(_currentAnim > 1)
            {
                Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
                gameObject.GetComponent<BoxCollider>().size = S;
                //gameObject.GetComponent<BoxCollider>().center = new Vector2(0, (S.y *2));
            }
            else
            {
                Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
                gameObject.GetComponent<BoxCollider>().size = S;
                //gameObject.GetComponent<BoxCollider>().center = new Vector2(0, (S.y));
            }
            _renderer.sprite = anim[_currentAnim];
        }
        else
        {
            _currentAnim = 0;
            _finishedSuttle = true;
        }
    }

    public void StartShuttle()
    {
        _finishedSuttle = false;
    }
}
