using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents a coin on the gameplay scene
/// </summary>
public class Coin : MonoBehaviour
{
    //Spinning animation array
    public Sprite[] anim;
    //Instanciated coin animation array
    public Sprite[] animInstantiated;
    //Spinning animation speed
    public float animSpeed = 0;
    //Instantiated animation speed
    public float animInstantiatedSpeed = 10;
    //bool for instantiated coins
    public bool _canMove = true;
   
    //animation time
    private float _time;
    //controls the current animation
    private int _currentAnim;
    //Renderer used to "draw" the sprites
    private SpriteRenderer _renderer;
    //Start position for the instantiated coin
    private Vector3 _startPosition;
    //Bool for instantiated coin
    private bool _down = false;
    private bool _isInstantiated = false;
    //Array with the animations to use (Instantiated or Spinning)
    private Sprite[] _animToUse;
    //Speed to use (Instantiated or Spinning)
    private float _speedToUse;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _startPosition = this.transform.position;
        if (_isInstantiated && animInstantiated.Length > 0)
        {
            _animToUse = animInstantiated;
            _speedToUse = animInstantiatedSpeed;
        }
        else
        {
            _animToUse = anim;
            _speedToUse = animSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CoinAnim();
        if (_canMove)
        {
            CoinMove();
        }
    }

    /// <summary>
    /// Animation of the gameobject with the anim array selected(Instantiated or Spinning), in this case represents a coin.
    /// </summary>
    private void CoinAnim()
    {
        _time += Time.deltaTime * _speedToUse;
        if (_time >= 1f)
        {
            _currentAnim++;
            _time = 0;
        }

        if (_currentAnim < anim.Length)
        {
            _renderer.sprite = _animToUse[_currentAnim];
        }
        else
        {
            _currentAnim = 0;
        }
    }

    /// <summary>
    /// Method used for moving an instantiated coin up and down. Finished this movement is destroyed.
    /// </summary>
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

    /// <summary>
    /// Setter for the moving flag
    /// </summary>
    /// <param name="canMove"></param>
    public void SetMove(bool canMove)
    {
        _canMove = canMove;
    }

    /// <summary>
    /// Destroys a gameobject when collision
    /// </summary>
    /// <param name="collision"></param>
    public void OnTriggerEnter(Collider collision)
    {
        if (!_canMove)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Setter for an instantiated coin
    /// </summary>
    public void SetInstantiated() {
        _isInstantiated = true;
    }

}
