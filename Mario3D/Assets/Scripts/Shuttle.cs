using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents a shuttle. When Mario collides with it, it impulses him up.
/// </summary>
public class Shuttle : MonoBehaviour
{
    //Animation array of the shuttle
    public Sprite[] anim;
    //Animation speed
    public float animSpeed = 0;

    //Animation time
    private float _time;
    //Current frame of the animation
    private int _currentAnim;
    //Renderer of the object to draw
    private SpriteRenderer _renderer;
    //Bool to check if the shuttling has finished
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
    }


    /// <summary>
    /// Represents the shuttle animation
    /// </summary>
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

    /// <summary>
    /// Setter of _finishedSuttle in order to shuttle again
    /// </summary>
    public void StartShuttle()
    {
        _finishedSuttle = false;
    }
}
