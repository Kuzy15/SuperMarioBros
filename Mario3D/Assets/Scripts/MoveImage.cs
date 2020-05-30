using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveImage : MonoBehaviour
{
    public Transform _objPosition;
    public Transform _endPos;

    private bool _startLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            this.transform.Translate(Vector3.right * 3.5f * Time.deltaTime);
            Color tmp = this.gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().color;
            tmp.a -= 0.004f;
            this.gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().color = tmp;
            if (this.transform.position.x > _endPos.position.x/4)
            {
                _startLoading = true;
            }
        }
    }

    public bool HasFinishedMovement()
    {
        return _startLoading;
    }

    public bool CanStartLoading()
    {
        return this.transform.position.x > _endPos.position.x;
    }

    public void ActiveImage()
    {
        _objPosition = GameObject.Find("Main Camera").transform.GetChild(0).transform;
        _endPos = GameObject.Find("Main Camera").transform.GetChild(1).transform;
        this.transform.position = new Vector3(_objPosition.position.x, _objPosition.position.y, 0);
        this.gameObject.SetActive(true);
        _startLoading = false;
        Color tmp = this.gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().color;
        tmp.a = 1f;
        this.gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().color = tmp;
    }
}
