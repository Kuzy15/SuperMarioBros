using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenKoopaTroopa : Enemy
{
    public Sprite[] reviveAnim;
    private Sprite[] aux;
    protected bool asd = false;
    protected bool fgh = false;

    public override void Die()
    {
        aux = anim;
        _dead = true;
        _renderer.sprite = animDead;
        StartCoroutine(Revive());     
    }

    IEnumerator Revive()
    {

        if (!asd)
        {
            _dead = false;
            Debug.Log("1");
            System.Array.Copy(reviveAnim, anim, 2);
            asd = true;
            yield return new WaitForSeconds(2.0f);
        }
        //else
        //{
        //    Debug.Log("2");
        //    anim = aux;
        //    yield return null;
        //}

    }
}
