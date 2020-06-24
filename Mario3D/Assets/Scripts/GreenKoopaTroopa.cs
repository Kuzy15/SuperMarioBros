using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenKoopaTroopa : Enemy
{
    public Sprite[] reviveAnim;

    protected bool _reviving = false;
    protected bool _isShell = false;

    private Sprite[] _aux;
    private bool _stopCoroutine = false;

    public override void Die()
    {
        if (!_reviving)
        {
            _reviving = true;
            _aux = anim;
            _dead = true;
            _canMove = false;
            _stopCoroutine = false;
            _collider.size = new Vector3(1.0f, 1.0f, 0.2f);
            _collider.center = new Vector3(0, 0.5f, 0);
            _renderer.sprite = animDead;
            _isShell = false;
            _velocity = 1.0f;
            positionShell = this.transform.position;
            StartCoroutine(Revive());
        }
        else
        {
            _dead = true;
            _canMove = true;
            _renderer.sprite = animDead;
            _velocity = 3.0f;
            _stopCoroutine = true;
            _dir *= -1;
            _reviving = false;
            _isShell = true;
           
        }
    }

    IEnumerator Revive()
    {
        yield return new WaitForSeconds(4.0f);
        if (!_stopCoroutine)
        {
            anim = reviveAnim;
            _dead = false;
            yield return new WaitForSeconds(0.5f);
       
            _collider.size = new Vector3(1.0f, 1.5f, 0.2f);
            _collider.center = new Vector3(0, 0.75f, 0);
            anim = _aux;
            yield return new WaitForSeconds(0.5f);

            _canMove = true;
        }
        yield return null;
    }
}
