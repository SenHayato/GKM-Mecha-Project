using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraActive : MonoBehaviour
{
    public GameObject Player;
    public GameObject Camera;

    public Transform cameraPivot;
    public Transform cameraAimPost;
    public Transform cameraMainPost;
    public PlayerInput cameraControl;
    public PlayerActive PlayerAct;
    private Camera MainCamera;
    private MechaPlayer Mecha;
    InputAction lookAction;

    [Header("ScopeCamera")]
    public float defaultFOV;
    public float scopeFOV;

    public float rotationSpeed;
    Vector2 lookInput;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        Mecha = FindAnyObjectByType<MechaPlayer>();

        MainCamera = Camera.GetComponent<Camera>();
        cameraControl = Player.GetComponent<PlayerInput>();
        cameraPivot = GetComponent<Transform>();
        lookAction = cameraControl.actions.FindAction("Look");
        PlayerAct = Player.GetComponent<PlayerActive>();

        cameraControl.enabled = true;

        //Default camera rotation
        cameraPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void ScopeCamera()
    {
        if (Mecha.isAiming)
        {
            MainCamera.fieldOfView = scopeFOV;
            MainCamera.transform.position = cameraAimPost.transform.position;
            SameRotation();
        }
        else
        {
            MainCamera.fieldOfView = defaultFOV;
            MainCamera.transform.position = cameraMainPost.transform.position;
            Vector3 adjustedRotation = Player.transform.eulerAngles;
            adjustedRotation.x = 0;
            Player.transform.eulerAngles = adjustedRotation; 
        }
    }

    public void ShootingCamera()
    {
        if (Mecha.isShooting && !Mecha.isAiming)
        {
            SameRotation();
            Vector3 adjustedRotation = Player.transform.eulerAngles;
            adjustedRotation.x = Mathf.Clamp(0f, -19f, 2.5f);
            Player.transform.eulerAngles = adjustedRotation;

        }
        else
        {
            Vector3 adjustedRotation = Player.transform.eulerAngles;
            adjustedRotation.x = 0;
            Player.transform.eulerAngles = adjustedRotation;
        }
    }

    public void SamePosition()
    {
        if (Player != null)
        {
            transform.position = Player.transform.position;
        }
    }

    public void SameRotation()
    {
        Player.transform.rotation = Camera.transform.rotation;
        
        // Mengatur ulang rotasi X ke 0 menggunakan Euler angles
        Vector3 adjustedRotation = Player.transform.eulerAngles;
        //adjustedRotation.x = 0;
        Player.transform.eulerAngles = adjustedRotation;
    }

    public void CameraRotation()
    {
        lookInput = lookAction.ReadValue<Vector2>();
        cameraPivot.transform.Rotate(Vector3.up, lookInput.x * rotationSpeed * Time.deltaTime);
        Vector3 currentRotation = cameraPivot.transform.localEulerAngles;
        float desiredXRotation = currentRotation.x - lookInput.y * rotationSpeed * Time.deltaTime;
        currentRotation.x = ClampAngle(desiredXRotation, -30f, 50f);
        currentRotation.z = 0;
        cameraPivot.transform.localEulerAngles = currentRotation;
        static float ClampAngle(float angle, float min, float max)
        {
            if (angle > 180f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }

    }

    private void Update()
    {
        SamePosition();
        CameraRotation();
    }
}
