using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creeper : MonoBehaviour
{
    public Sprite[] anim;
    public float animSpeed = 0;


    private float _time;
    private int _currentAnim;
    private SpriteRenderer _renderer;
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

    private void CreeperAnim()
    {
        _time += Time.deltaTime * animSpeed;
        if (_time >= 1f)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = _canFlip;
            _time = 0;
            _canFlip = !_canFlip;
        }

        /*if (_currentAnim < anim.Length)
        {
            _renderer.sprite = anim[_currentAnim];
        }
        else
        {
            _currentAnim = 0;
        }*/
    }
}