using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents the camera used in the gameplay scene. This camera follows Mario in the X axis
/// </summary>
public class GameCamera : MonoBehaviour
{
    //Gamecamera target
    public Transform target;
    //Bool for following the player
    public bool followPlayer = true;
    //Distance to the player
    public Vector3 offset;
    //Following speed
    public float smoothSpeed = 10f;
    //Variables of the looking mode
    public float zoomSpeed;
    public float targetOrtho;
    public float orthoSize;
    public float smoothSpeed2 = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;
    public int scrollSpeed = 8;
    //Camera reference postion
    private Vector3 _refPosition;
    //Get if is in looking mode
    private bool _looking = false;
    private Vector3 _lastPos = Vector3.zero;
    //Saves the initial color of the background
    private Color _initialColor;
    private bool _canFollowInY = false;

    //Instance of this class
    private static GameCamera _camera = null;

    public static GameCamera Instance
    {
        get
        {
            return _camera;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        targetOrtho = Camera.main.orthographicSize;
        orthoSize = targetOrtho;
        _refPosition = this.transform.position;
        _initialColor = this.GetComponent<Camera>().backgroundColor;
        this.transform.position = new Vector3(this.transform.position.x + 3,-16.5f, this.transform.position.z);
        //GoToBlackScreen();
    }

    // Update is called once per frame
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

    /// <summary>
    /// Resets the camera to its reference position
    /// </summary>
    public void ResetCamera()
    {
        Camera.main.transform.position = _refPosition;
        StartCoroutine("Coroutine");
    }


    /// <summary>
    /// Coroutine that allows camera to exit looking mode and reset its initial size
    /// </summary>
    /// <returns></returns>
    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(0.3f);
        _looking = false;
        targetOrtho = Camera.main.orthographicSize = orthoSize;
    }

    /// <summary>
    /// Setter of looking mode
    /// </summary>
    private void SetLookingMode()
    {
        _looking = true;
    }

    /// <summary>
    /// Getter of looking mode
    /// </summary>
    /// <returns></returns>
    public bool GetLooking()
    {
        return _looking;
    }

    /// <summary>
    /// Setter of the y position of the camera
    /// </summary>
    /// <param name="y">Y position</param>
    public void SetCameraY(float y)
    {
        this.transform.position = new Vector3(this.transform.position.x, y, this.transform.position.z);
    }

    /// <summary>
    /// Setter of the x position of the camera
    /// </summary>
    /// <param name="x"></param>
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

    //If has a set target, camera follows it with a determined offset
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

    /// <summary>
    /// This method allows us to check if an object is visible to camera.
    /// If visible, it can move, if not, it will be idle
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }


    /// <summary>
    /// Camera goes to black. Used for entering and exiting secret zones
    /// </summary>
    public void GoToBlackScreen()
    {
        this.GetComponent<Camera>().backgroundColor = Color.black;
    }

    /// <summary>
    /// Camera goes to blue, its initial color. Used for reset camera background when exiting secret zones
    /// </summary>
    public void GoToBlueScreen()
    {
        this.GetComponent<Camera>().backgroundColor = _initialColor;
    }


    /// <summary>
    /// Getter of the y position of the camera
    /// </summary>
    /// <returns></returns>
    public float GetCameraY()
    {
        return this.transform.position.y;
    }


    /// <summary>
    /// Getter to see if camera can follow in Y
    /// </summary>
    /// <param name="can"></param>
    public void CanFollowInY(bool can)
    {
        _canFollowInY = can;
    }
}
