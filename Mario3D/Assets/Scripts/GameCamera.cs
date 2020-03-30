using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameCamera : MonoBehaviour
{
    public Transform target;
    public bool followPlayer = true;
    public Vector3 offset;
    public float smoothSpeed = 10f;
    private Vector3 lastPos = Vector3.zero;
    private float halfPlayerSizeX;

    public float zoomSpeed;
    public float targetOrtho;
    public float orthoSize;
    public float smoothSpeed2 = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;
    public int scrollSpeed = 8;
    
    private Vector3 refPosition;
    bool sett = false;

    private static GameCamera _camera = null;

    private bool looking = false;
    private bool canReset = false;

    public static GameCamera Instance
    {
        get
        {

            return _camera;
        }
    }

    void Start()
    {
        halfPlayerSizeX = target.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        targetOrtho = Camera.main.orthographicSize;
        orthoSize = targetOrtho;
        refPosition = this.transform.position;
    }

    private void Update()
    {
        if (target != null)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                refPosition = Camera.main.transform.position;
                SetLookingMode();
            }
            if (looking)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0.0f)
                {
                    targetOrtho -= scroll * zoomSpeed;
                    targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
                }
                sett = false;

                Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed2 * Time.deltaTime);
                float dir = Input.GetAxis("Horizontal");
                float vDir = Input.GetAxis("Vertical");
                transform.Translate(new Vector3(dir * scrollSpeed * Time.deltaTime, vDir * scrollSpeed * Time.deltaTime, transform.position.z));
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Camera.main.transform.position = refPosition;
                    StartCoroutine("coroutine");
                }

            }
            else
            {
                targetOrtho = Camera.main.orthographicSize = orthoSize;
                //sett = false;
                /*if (canReset)
                {
                    Camera.main.transform.position = refPosition;
                    //canReset = false;
                }*/
            }
        }
    }

    IEnumerator coroutine()
    {
        yield return new WaitForSeconds(0.3f);
        looking = false;
        targetOrtho = Camera.main.orthographicSize = orthoSize;
    }

    private void SetLookingMode()
    {
        looking = true;
    }

    public bool GetLooking()
    {
        return looking;
    }

    private void Awake()
    {
        if (_camera != null && _camera != this)
        {
            //Destroy(this.gameObject);
        }

        _camera = this;
        //DontDestroyOnLoad(this.gameObject);
        lastPos = transform.position;
    }

    //Si tiene un target establecido, entonces la camara se encargará de seguirle, con un determinado offset.
    void LateUpdate()
    {
        if (target != null)
        {
            if (!looking)
            {
                if (followPlayer && (target.position - lastPos).x > 0)
                {
                    Vector3 desiredPosition = target.position + offset;
                    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
                    transform.position = new Vector3(smoothedPosition.x, transform.position.y, transform.position.z);
                    lastPos = transform.position;
                }
                else
                {
                    ////Debug.Log("AHHAHAHA");
                    //canReset = true;
                }
            }
        }
       // clampPlayerMovement();
    }

    void clampPlayerMovement()
    {
        Vector3 position = transform.position;

        float distance = transform.position.z - Camera.main.transform.position.z;

        float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x + halfPlayerSizeX;
        float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance)).x - halfPlayerSizeX;

        position.x = Mathf.Clamp(position.x, leftBorder, rightBorder);
        transform.position = position;
    }

    public void SetCanReset()
    {
        canReset = true;
    }

    public void InactiveCanReset()
    {
        canReset = false;
        //sett = false;
    }

    public bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
