using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameCamera : MonoBehaviour
{
    public Transform target;
    public bool followPlayer = true;
    public Vector3 offset;
    public float smoothSpeed = 10f;
    public float zoomSpeed;
    public float targetOrtho;
    public float orthoSize;
    public float smoothSpeed2 = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;
    public int scrollSpeed = 8;
    
    private Vector3 _refPosition;
    private bool _looking = false;
    private Vector3 _lastPos = Vector3.zero;
    private float _halfPlayerSizeX;
    private Color _initialColor;
    private bool _canFollowInY = false;

    private static GameCamera _camera = null;

    public static GameCamera Instance
    {
        get
        {
            return _camera;
        }
    }

    void Start()
    {
        _halfPlayerSizeX = target.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        targetOrtho = Camera.main.orthographicSize;
        orthoSize = targetOrtho;
        _refPosition = this.transform.position;
        _initialColor = this.GetComponent<Camera>().backgroundColor;
        this.transform.position = new Vector3(this.transform.position.x + 3,-16.5f, this.transform.position.z);
        //GoToBlackScreen();
    }

    private void Update()
    {
        if (target != null)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _refPosition = Camera.main.transform.position;
                SetLookingMode();
            }
            if (_looking)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0.0f)
                {
                    targetOrtho -= scroll * zoomSpeed;
                    targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
                }

                Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed2 * Time.deltaTime);
                float dir = Input.GetAxis("Horizontal");
                float vDir = Input.GetAxis("Vertical");
                transform.Translate(new Vector3(dir * scrollSpeed * Time.deltaTime, vDir * scrollSpeed * Time.deltaTime, transform.position.z));

                if (Input.GetKeyDown(KeyCode.R))
                {
                    Camera.main.transform.position = _refPosition;
                    StartCoroutine("Coroutine");
                }

            }
            else
            {
                targetOrtho = Camera.main.orthographicSize = orthoSize;            
            }
        }
    }

    public void ResetCamera()
    {
        Camera.main.transform.position = _refPosition;
        StartCoroutine("Coroutine");
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(0.3f);
        _looking = false;
        targetOrtho = Camera.main.orthographicSize = orthoSize;
    }

    private void SetLookingMode()
    {
        _looking = true;
    }

    public bool GetLooking()
    {
        return _looking;
    }

    public void SetCameraY(float y)
    {
        this.transform.position = new Vector3(this.transform.position.x, y, this.transform.position.z);
    }


    public void SetCameraX(float x)
    {
        this.transform.position = new Vector3(x, this.transform.position.y, this.transform.position.z);
    }

    private void Awake()
    {
        if (_camera != null && _camera != this)
        {
            //Destroy(this.gameObject);
        }

        _camera = this;
        //DontDestroyOnLoad(this.gameObject);
        _lastPos = transform.position;
    }

    //Si tiene un target establecido, entonces la camara se encargará de seguirle, con un determinado offset.
    void LateUpdate()
    {
        if (target != null)
        {
            if (!_looking)
            {
                if (followPlayer && (target.position - _lastPos).x > 0)
                {
                    Vector3 desiredPosition = target.position + offset;
                    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
                    float posY;
                    if (_canFollowInY)
                    {
                        posY = smoothedPosition.y;
                    }
                    else
                    {
                        posY = this.transform.position.y;
                    }
                    transform.position = new Vector3(smoothedPosition.x, posY, transform.position.z); ;
                    _lastPos = transform.position;
                }             
            }
        }

    }

    public bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    public void GoToBlackScreen()
    {
        this.GetComponent<Camera>().backgroundColor = Color.black;
    }

    public void GoToBlueScreen()
    {
        this.GetComponent<Camera>().backgroundColor = _initialColor;
    }

    public float GetCameraY()
    {
        return this.transform.position.y;
    }

    public void CanFollowInY(bool can)
    {
        _canFollowInY = can;
    }
}
