using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimator : MonoBehaviour
{

    //Animation time
    private float _time;
    //Current frame of the animation
    private int _currentAnim = 0;
    //Renderer of the enemy
    protected SpriteRenderer _renderer;

    //Animation array of the enemy
    public Sprite[] anim;
    //Speed animation
    public float animSpeed = 4;


    // Start is called before the first frame update
    void Start()
    {
     _renderer = this.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimEntity();
    }

    private void AnimEntity()
    {
        _time += Time.deltaTime * animSpeed;
        if (_time >= 1f)
        {
            ////Debug.Log("anim");
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
