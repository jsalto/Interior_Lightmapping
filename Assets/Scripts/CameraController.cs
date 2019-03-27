using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    public Slider sliderSpeed = null;
    public float speedMultiplier = 2.5F;
    public float MovementSpeed = 0.05F;
    public float UpSpeed = 0.5F;
    public float RotationSpeed = 0.07F;
    public float Zoom2DSpeed = 1;
    public float minPitch = 1.0f;
    public float maxPitch = -1.0f;
    public float pitch = 0.0f;
    public Camera myCamera;
    public Camera myCameraLeap;

    Transform _myTransform;
    //GameObject _myGameObject;
    //Transform _myCameraTransform;

    public bool movementLocked = false;
    public bool cameraMoving = false;
    public bool isOutside = true;
    public bool canUseArrows = true;

    [HideInInspector]
    public bool autoMovement = false;
    [HideInInspector]
    public bool autozoom = false;

    Vector3 autoDirection = new Vector3();
    Vector3 startPos = new Vector3();
    Vector3 finalPos = new Vector3();

    float startSize = 0;
    float finalSize = 0;
    float intervalTime = 0;

    public Vector3 originalPosition = Vector3.zero;
    public Quaternion originalRotation = Quaternion.identity;
    public bool hasOriginalPos = false;
    public bool currentActive = true;
    public bool is2D = false;
    
    public Vector3 CurrentPosition
    {
        get { return _myTransform.position; }
    }

    void Awake()
    {
        _myTransform = transform;
        //_myGameObject = gameObject;
        //_myCameraTransform = myCamera.transform;

        //if(PreferencesManager.MandosActivos.used) {
        //    if(PreferencesManager.MandosActivos.Value == 1 && myCameraLeap != null) {
        //        _myCameraTransform = myCameraLeap.transform;
        //    }
        //}
    }

    public void SetCamera(Camera newCamera)
    {
        myCamera = newCamera;
        //_myCameraTransform = newCamera.transform;
    }

	void Start () 
    {
        //if (PreferencesManager.VelocidadMovimiento.used)
        //{
        //    if (sliderSpeed != null)
        //    {
        //        sliderSpeed.value = PreferencesManager.VelocidadMovimiento.Value;
        //        speedMultiplier = sliderSpeed.value;
        //    }
        //}
	}

    public void SliderSpeedChange()
    {
        if (sliderSpeed != null)
        {
            speedMultiplier = sliderSpeed.value;
            //PreferencesManager.VelocidadMovimiento.Value = speedMultiplier;
        }
    }

    public void CameraReset()
    {
        autozoom = false;
        autoMovement = false;
        cameraMoving = false;
        intervalTime = 0;
    }

    public Ray GetCameraScreenToWorldRay(Vector3 pos)
    {
        Ray retRay = new Ray();
        if (myCamera != null)
        {
            retRay = myCamera.ScreenPointToRay(pos);
        }

        return retRay;
    }

    public void ForcePosition(Vector3 pos)
    {
        _myTransform.position = pos;
        //CameraManager.instance.bCameraPositionSet = true;
    }

    public void ForcePositionRotation(Vector3 pos, Vector3 rotationAngles)
    {
        _myTransform.position = pos;
        _myTransform.localRotation = Quaternion.Euler(rotationAngles.x, rotationAngles.y, rotationAngles.z);
        //CameraManager.instance.bCameraPositionSet = true;
    }

    void Update () 
    {
        //if (currentActive && (!CameraManager.instance || (CameraManager.instance && CameraManager.instance.cameraMode == CameraManager.CameraModes.CLASSIC)))
        {
            //if (!XboxControllerInputCheck() && !VRInputCheck()) 
            {
                //Keyboard input
                //if (!movementLocked && !VRManager.isVR && !AppLogic.takingPhoto && !AppLogic.panelVideoActive && !AppLogic.editingText && canUseArrows)
                //{
                    GetKeyboardInput();
                //}
                //Mouse input
               // if (!movementLocked && (EventSystem.current == null || (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())) && !AppLogic.takingPhoto && !AppLogic.panelVideoActive && !AppLogic.editingText && (Interface3DLogic.instance == null || !Interface3DLogic.instance.mouseOverInterface))
                {
                    cameraMoving = false;

                    GetMouseInput();
                    GetTouchInput();
                    
                    if (autoMovement)
                    {
                        float multiplier = is2D ? 2 : 20;
                        _myTransform.position += multiplier * autoDirection.normalized * MovementSpeed * speedMultiplier * Time.deltaTime;
                    }
                    if (autozoom)
                    {
                        intervalTime += Time.deltaTime;
                        if (intervalTime >= 1)
                        {
                            intervalTime = 1;
                            autozoom = false;
                        }
                        if (is2D)
                        {
                            myCamera.orthographicSize = startSize + (intervalTime * (finalSize - startSize));
                        }
                        else
                        {
                            Vector3 cameraPos = Vector3.Lerp(startPos, finalPos, intervalTime);
                            _myTransform.position = cameraPos;
                        }
                    }
                }
            }
        }
	}

    void GetMouseInput()
    {
        if(Input.GetMouseButton(0)) // Left button clicked
        {
            CameraReset();
            cameraMoving = true;
            if(is2D) {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                Vector3 directionX = new Vector3(_myTransform.right.x, 0, _myTransform.right.z);
                directionX.Normalize();
                Vector3 directionY = new Vector3(_myTransform.up.x, _myTransform.up.y, _myTransform.up.z);
                directionY.Normalize();
                Vector3 directionXY = mouseY * directionY + mouseX * directionX;
                _myTransform.position += directionXY * (MovementSpeed * myCamera.orthographicSize) * Time.deltaTime;
            }
            else {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                //Vector3 directionX = new Vector3(_myTransform.right.x, 0, _myTransform.right.z);
                //directionX.Normalize();
                Vector3 directionY = new Vector3(_myTransform.forward.x, 0, _myTransform.forward.z);
                directionY.Normalize();
                Vector3 directionXY = mouseY * directionY;
                _myTransform.position += directionXY * MovementSpeed * speedMultiplier /* Time.deltaTime*/;
                _myTransform.Rotate(Vector3.up, mouseX * RotationSpeed * 10 * speedMultiplier /* Time.deltaTime*/, Space.World);
            }
        }
        if(Input.GetMouseButton(1)) // Right button clicked
        {
            CameraReset();
            cameraMoving = true;
            if(!is2D) {
                //   float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                Quaternion lastRotation = _myTransform.rotation;
                _myTransform.Rotate(_myTransform.right, mouseY * RotationSpeed * 10 * speedMultiplier /* Time.deltaTime*/, Space.World);

                /*roll = Mathf.Atan2(2 * y * w - 2 * x * z, 1 - 2 * y * y - 2 * z * z);
                pitch = Mathf.Atan2(2 * x * w - 2 * y * z, 1 - 2 * x * x - 2 * z * z);
                yaw = Mathf.Asin(2 * x * y + 2 * z * w);*/

                float yValue = 2 * _myTransform.rotation.x * _myTransform.rotation.w - 2 * _myTransform.rotation.y * _myTransform.rotation.z;
                float xValue = 2 * _myTransform.rotation.x * _myTransform.rotation.x - 2 * _myTransform.rotation.z * _myTransform.rotation.z;
                pitch = Mathf.Atan2(yValue, xValue);

                // camara inclinada arriba - pitch negativo
                // camara inclinada abajo - pitch positivo
                // cercano a sin inclinacion - pitch aprox 1.5 o -1.5
                // cercano a la inclinacion maxima aprox 0.9 o -0.9
                if((pitch > 0 && pitch < minPitch) || (pitch < 0 && pitch > maxPitch)) {
                    _myTransform.rotation = lastRotation;
                }
            }
        }
        if(is2D) {
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            myCamera.orthographicSize -= Zoom2DSpeed * mouseWheel;
            if(myCamera.orthographicSize < 0.1)
                myCamera.orthographicSize = 0.1f;
        }
        else {
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if(mouseWheel > 0 || mouseWheel < 0 && _myTransform.position.y > 0.05) {
                CameraReset();
                cameraMoving = true;
                _myTransform.position += Vector3.up * mouseWheel * UpSpeed;
                if(_myTransform.position.y < 0.05) {
                    _myTransform.position = new Vector3(_myTransform.position.x, 0.05f, _myTransform.position.z);
                }
            }
        }
    }

    void GetTouchInput()
    {
        switch (Input.touchCount)
        {
            case 0:
                break;
            case 1:
                {
                    CameraReset();
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        cameraMoving = true;
                        Vector2 deltaPos = Input.GetTouch(0).deltaPosition;

                        Vector3 directionY = new Vector3(_myTransform.forward.x, 0, _myTransform.forward.z);
                        directionY.Normalize();
                        Vector3 directionXY = deltaPos.y * directionY;
                        _myTransform.position += directionXY * MovementSpeed * speedMultiplier * 0.05f/* Time.deltaTime*/;
                        _myTransform.Rotate(Vector3.up, deltaPos.x * RotationSpeed * 10 * speedMultiplier * 0.05f/* Time.deltaTime*/, Space.World);
                    }
                }
                break;
            case 2:
                {
                    CameraReset();
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        cameraMoving = true;
                        Vector2 deltaPos = Input.GetTouch(0).deltaPosition;
                        Quaternion lastRotation = _myTransform.rotation;
                        _myTransform.Rotate(_myTransform.right, -deltaPos.y * RotationSpeed * 10 * speedMultiplier * 0.05f/* Time.deltaTime*/, Space.World);

                        float yValue = 2 * _myTransform.rotation.x * _myTransform.rotation.w - 2 * _myTransform.rotation.y * _myTransform.rotation.z;
                        float xValue = 2 * _myTransform.rotation.x * _myTransform.rotation.x - 2 * _myTransform.rotation.z * _myTransform.rotation.z;
                        pitch = Mathf.Atan2(yValue, xValue);

                        if ((pitch > 0 && pitch < minPitch) || (pitch < 0 && pitch > maxPitch))
                        {
                            _myTransform.rotation = lastRotation;
                        }
                    }
                }
                break;
            case 3:
                {
                    CameraReset();
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        cameraMoving = true;
                        Vector2 deltaPos = Input.GetTouch(0).deltaPosition;
                        _myTransform.position += Vector3.up * deltaPos.y * UpSpeed * 0.01f;
                        if (_myTransform.position.y < 0.05)
                        {
                            _myTransform.position = new Vector3(_myTransform.position.x, 0.05f, _myTransform.position.z);
                        }
                    }
                }
                break;
        }
    }

    void GetKeyboardInput()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)) {
            autozoom = false;
            Vector3 newAutoDirection = new Vector3(_myTransform.forward.x, 0, _myTransform.forward.z);
            if(is2D)
                newAutoDirection = new Vector3(_myTransform.up.x, 0, _myTransform.up.z);
            if(autoDirection == newAutoDirection) {
                autoMovement = !autoMovement;
            }
            else {
                autoMovement = true;
            }
            autoDirection = newAutoDirection;

        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            autozoom = false;
            Vector3 newAutoDirection = new Vector3(-_myTransform.forward.x, 0, -_myTransform.forward.z);
            if(is2D)
                newAutoDirection = new Vector3(-_myTransform.up.x, 0, -_myTransform.up.z);
            if(autoDirection == newAutoDirection) {
                autoMovement = !autoMovement;
            }
            else {
                autoMovement = true;
            }
            autoDirection = newAutoDirection;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            autozoom = false;
            Vector3 newAutoDirection = new Vector3(_myTransform.right.x, 0, _myTransform.right.z);
            if(autoDirection == newAutoDirection) {
                autoMovement = !autoMovement;
            }
            else {
                autoMovement = true;
            }
            autoDirection = newAutoDirection;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            autozoom = false;
            Vector3 newAutoDirection = new Vector3(-_myTransform.right.x, 0, -_myTransform.right.z);
            if(autoDirection == newAutoDirection) {
                autoMovement = !autoMovement;
            }
            else {
                autoMovement = true;
            }
            autoDirection = newAutoDirection;
        }
        else if(Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) {
            if(is2D) {
                startSize = myCamera.orthographicSize;
                finalSize = myCamera.orthographicSize + 2;
            }
            else {
                startPos = _myTransform.position;
                finalPos = _myTransform.position + _myTransform.forward * 2;
            }
            CameraReset();
            autozoom = true;
        }
        else if(Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) {
            if(is2D) {
                startSize = myCamera.orthographicSize;
                finalSize = myCamera.orthographicSize - 2;
                if(finalSize < 0.1)
                    finalSize = 0.1f;
            }
            else {
                startPos = _myTransform.position;
                finalPos = _myTransform.position - _myTransform.forward * 2;
            }
            CameraReset();
            autozoom = true;
        }
    }
}
