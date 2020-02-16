using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBlock : MonoBehaviour
{

    private float _time;
    private int _currentAnim;
    private SpriteRenderer _renderer;
    private Vector2 _startPosition;
    private bool _active = false;
    private bool _disable = false;
    private bool _goDown = false;
    private bool _dropped = false;

    public Sprite[] anim;
    public Sprite disableBlock;
    public float animSpeed = 0;
    public GameObject entity;

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = this.transform.position;
        _renderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        BounceMove();

        MagicBlockAnim();

        MoveBlock();
    }


    private void BounceMove()
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


    private void MagicBlockAnim()
    {
        if (!_disable)
        {
            _time += Time.deltaTime * animSpeed;
            if (_time >= 1.0f)
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
    }


    private void MoveBlock()
    {
        if (_active)
        {

            if (entity && !_disable)
            {
                _disable = true;
                InstantiateEntity();
            }

            _renderer.sprite = disableBlock;
            
        }
    }


    private void InstantiateEntity()
    {
        Instantiate(entity, _startPosition,Quaternion.identity);
    }

    


    public void ActivateBlock()
    {
        if (!_dropped)
        {
            _dropped = true;
            _active = true;
        }
    }

}
