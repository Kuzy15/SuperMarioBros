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
        _dir *= -1;
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
    }

    IEnumerator Revive()
    {
        _canMove = false;
        /*_aux = anim;
        anim[0] = animDead;
        anim[1] = animDead;*/
        yield return new WaitForSeconds(4.0f);
        _animateOnDie = true;
        if (!_stopCoroutine)
        {
            anim = reviveAnim;
            yield return new WaitForSeconds(2.5f);

            _collider.size = new Vector3(1.0f, 1.5f, 0.2f);
            _collider.center = new Vector3(0, 0.75f, 0);
            anim = _aux;

            _dead = false;
            _canMove = true;
            _animateOnDie = false;
            anim = _aux;
            _reviving = false;
        }
        yield return null;
    }
}