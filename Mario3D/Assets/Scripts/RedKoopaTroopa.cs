using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedKoopaTroopa : GreenKoopaTroopa
{
    public float range = 0;

    public override void EnemyMove()
    {
        transform.Translate(new Vector3(1.0f * _dir, 0) * Time.deltaTime);

        if (!_isShell)
        {
            RaycastHit hit;

            Debug.DrawRay(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.left * _dir), Color.yellow);

            if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.right), out hit, 0.5f) ||
                Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.left), out hit, 0.5f))
            {
                if (hit.transform.gameObject.tag != "Player")
                    _dir *= -1;
            }

            if (positionShell.x - range >= transform.position.x || positionShell.x <= transform.position.x)
            {
                _renderer.flipX = !_renderer.flipX;
                _dir *= -1;
            }
        }
    }
}
