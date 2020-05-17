using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that reprsents a single creeper
/// </summary>
public class Creeper : MonoBehaviour
{
    //Animation array
    public Sprite[] anim;
    //Animation speed
    public float animSpeed = 0;

    //Animation time
    private float _time;
    //Current frame of the animation
    private int _currentAnim;
    //Renderer of the object
    private SpriteRenderer _renderer;
    //Bool to check if the creeper can flip
    private bool _canFlip = true;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CreeperAnim();
    }
    /// <summary>
    /// CreeperAnimation
    /// </summary>
    private void CreeperAnim()
    {
        _time += Time.deltaTime * animSpeed;
        if (_time >= 1f)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = _canFlip;
            _time = 0;
            _canFlip = !_canFlip;
        }
    }
}