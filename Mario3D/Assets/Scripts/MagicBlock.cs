using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Method that represents a magic block. It can instantiate a mushroom or a coin, when player collision
/// </summary>
public class MagicBlock : MonoBehaviour
{
    //Bounce animation of the block
    public Sprite[] _upAnim;
    //Entity containing this class
    public GameObject entity;
    //Animation time
    private float _time;
    //Current frame of the animation
    private int _currentAnim;
    //Renderer to "draw" the object
    private SpriteRenderer _renderer;
    //Start position of the block, to make it bounce
    private Vector2 _startPosition;
    //Bools to check if has already dropped an object and to see if it has bounced
    private bool _active = false;
    private bool _disable = false;
    private bool _goDown = false;
    private bool _dropped = false;
    //Array with all the animation frames
    private Sprite[] _anim;
    //Sprite to put when an object has been dropped from the block
    private Sprite _disableBlock;
    //Animation speed
    private float _animSpeed = 0;


    // Start is called before the first frame update
    void Start()
    {

        FillMagicBlock();
        _startPosition = this.transform.position;
        _renderer = this.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Fills the current block with the animation, disable sprite, and the entinty that instantiates
    /// </summary>
    void FillMagicBlock()
    {
        _anim = MagicBlockManager.GM.GetAnim();
        _disableBlock = MagicBlockManager.GM.GetDisableBlock();
        _animSpeed = MagicBlockManager.GM.GetAnimSpeed();
        entity = MagicBlockManager.GM.SetEntity();
    }

    // Update is called once per frame
    void Update()
    {
        BounceMove();

        MagicBlockAnim();

        MoveBlock();
    }

    /// <summary>
    /// Function that calculates the distance from an start position to an end point. On equal distance, the block is allowed to go down.
    /// Represents the whole bouncing movement.
    /// </summary>
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

    /// <summary>
    /// Animation of the magic block
    /// </summary>
    private void MagicBlockAnim()
    {
        if (!_disable)
        {
            _time += Time.deltaTime * _animSpeed;
            if (_time >= 1.0f)
            {
                _currentAnim++;
                _time = 0;
            }

            if (_currentAnim < _anim.Length)
            {
                _renderer.sprite = _anim[_currentAnim];
            }
            else
            {
                _currentAnim = 0;
            }
        }
    }

    /// <summary>
    /// Check if the current block is active in order to see if an object can be instantiated
    /// </summary>
    private void MoveBlock()
    {
        if (_active)
        {

            if (entity && !_disable)
            {
                _disable = true;
                InstantiateEntity();
            }

            _renderer.sprite = _disableBlock;

        }
    }

    /// <summary>
    /// Instantiates an entity on the magic block position
    /// </summary>
    private void InstantiateEntity()
    {
        if (entity.GetComponent<Coin>())
        {
            entity.GetComponent<Coin>().SetInstantiated();
            entity.GetComponent<Coin>().SetMove(true);
        }
        Instantiate(entity, _startPosition, Quaternion.identity);
    }

    /// <summary>
    /// Activates the block, once the player has collided with it.
    /// </summary>
    public void ActivateBlock()
    {
        if (!_dropped)
        {
            _dropped = true;
            _active = true;
        }
    }
}
