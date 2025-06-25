using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraActive : MonoBehaviour
{
    public GameObject Player;
    //public GameObject Camera;
    public WeaponRaycast weaponSkrip;
    [SerializeField] LayerMask collisionLayers;

    [Header("CameraPivotPost")]
    public Transform cameraPivot; //posisi disamakan dengan player
    public Transform cameraAimPost;
    public Transform cameraMainPost;
    [SerializeField] Transform defaultMainPost;
    //public Transform cameraParent; //untuk akses player active saja

    [Header("Reference")]
    public PlayerInput cameraControl;
    public PlayerActive PlayerAct;
    public GameObject MainCameraOBJ;
    public Camera MainCamera;
    [SerializeField] Animation mainCameraAnimation;
    [SerializeField] MechaPlayer Mecha;
    [SerializeField] GameObject collisionPoint;
    InputAction lookAction;

    [Header("ScopeCamera")]
    public float defaultFOV;
    public float scopeFOV;
    public float boostFOV;

    [Header("Camera Value")]
    public float rotationSpeed;
    [SerializeField] float recoilLerp; //default 0.1f
    Vector2 lookInput;
    public Quaternion defaultCamRot;
    [SerializeField, Range(0f, 5f)] float collisionOffset;
    [SerializeField, Range(0f, 20f)] float offsetSmooth;
    
    //Flag
    private Vector3 currentRecoil;
    private float currentLerpTime = 0f;
    private bool isAiming = false;
    public bool mechaInAwakenState = false;

    private void Awake()
    {
        weaponSkrip = FindObjectOfType<WeaponRaycast>();
        Player = GameObject.FindGameObjectWithTag("Player");
        Mecha = FindAnyObjectByType<MechaPlayer>();
        cameraControl = FindAnyObjectByType<PlayerInput>();
        PlayerAct = Player.GetComponent<PlayerActive>();
        collisionPoint = GameObject.FindGameObjectWithTag("CollisionPoint");
    }
    private void Start()
    {
        cameraAimPost = GameObject.FindGameObjectWithTag("AimCameraPosition").transform;
        MainCamera = MainCameraOBJ.GetComponentInChildren<Camera>();
        lookAction = cameraControl.actions.FindAction("Look");
        MainCamera.transform.position = cameraMainPost.transform.position;
    }

    public void CameraCollision()
    {
        Vector3 targetPosition;
        if (Physics.Linecast(defaultMainPost.transform.position, collisionPoint.transform.position, out RaycastHit hitinfo, collisionLayers))
        {
            //Debug.DrawLine(defaultMainPost.transform.position, collisionPoint.transform.position, Color.red);
            Debug.Log("Camera nabrak");

            Vector3 offset = new(0f, 0f, collisionOffset); //Agar tidak terlalu masuk ke dalam
            targetPosition = hitinfo.point + offset;
            //cameraMainPost.transform.position = targetPosition;
            cameraMainPost.transform.position = Vector3.Lerp(cameraMainPost.transform.position, targetPosition, Time.deltaTime * offsetSmooth);
        }
        else
        {
            targetPosition = defaultMainPost.transform.position;
            cameraMainPost.transform.position = Vector3.Lerp(cameraMainPost.transform.position, targetPosition, Time.deltaTime * offsetSmooth);
        }
    }

    public void ScopeCamera()
    {
        float lerpDuration = 0.05f;
        if (Mecha.isAiming && !Mecha.isBoosting)
        {
            if (!isAiming)
            {
                currentLerpTime = 0f;
                isAiming = true;
            }
            currentLerpTime += Time.deltaTime / lerpDuration;
            currentLerpTime = Mathf.Clamp01(currentLerpTime);
            MainCamera.fieldOfView = Mathf.Lerp(defaultFOV, scopeFOV, Mathf.SmoothStep(0f, 1f, currentLerpTime));
            MainCamera.transform.position = Vector3.Lerp(cameraMainPost.position, cameraAimPost.position, Mathf.SmoothStep(0f, 1f, currentLerpTime));
            SameRotation();
        }
        else
        {
            if (isAiming)
            {
                currentLerpTime = 0f;
                isAiming = false;
            }

            currentLerpTime += Time.deltaTime / lerpDuration;
            currentLerpTime = Mathf.Clamp01(currentLerpTime);
            MainCamera.fieldOfView = Mathf.Lerp(scopeFOV, defaultFOV, Mathf.SmoothStep(0f, 1f, currentLerpTime));
            MainCamera.transform.position = Vector3.Lerp(cameraAimPost.position, cameraMainPost.position, Mathf.SmoothStep(0f, 1f, currentLerpTime));
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

    public IEnumerator RecoilEffect()
    {
        float recoilUp = weaponSkrip.recoilValueX; // Recoil selalu naik
        float recoilSide = Random.Range(-weaponSkrip.recoilValueY, weaponSkrip.recoilValueY); // Acak ke samping
        float recoilSpeed = weaponSkrip.recoilSpeed;
        float elapsedTime = 0f;
        currentRecoil += new Vector3(recoilSide, recoilUp, 0); // Tambahkan recoil baru

        // Recoil naik
        while (elapsedTime < recoilLerp)
        {
            elapsedTime += Time.deltaTime * recoilSpeed;
            Quaternion targetRotation = Quaternion.Euler(-currentRecoil.y, currentRecoil.x, 0f);
            MainCameraOBJ.transform.localRotation = Quaternion.Lerp(MainCameraOBJ.transform.localRotation, targetRotation, elapsedTime / recoilLerp);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        elapsedTime = 0f;

        // posisi awal
        while (weaponSkrip.ammo <= 0 || !Mecha.isShooting)
        {
            elapsedTime += Time.deltaTime * recoilSpeed;
            currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, elapsedTime / 0.1f);
            Quaternion targetRotation = Quaternion.Euler(-currentRecoil.y, currentRecoil.x, 0f);
            MainCameraOBJ.transform.localRotation = targetRotation;
            yield return null;
        }
    }

    public void SamePosition()
    {
        if (Player != null)
        {
            cameraPivot.transform.position = Player.transform.position;
        }
    }

    public void SameRotation()
    {
        Player.transform.rotation = MainCameraOBJ.transform.rotation;
        Vector3 adjustedRotation = Player.transform.eulerAngles;
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

    bool wasAwaken = false;
    public void AwakeningCameraPost()
    {
        if (!wasAwaken)
        {
            wasAwaken = true;
            mainCameraAnimation.Play("AwakeningCameraPosition");
        }
    }

    private void Update()
    {
        if (!mechaInAwakenState)
        {
            wasAwaken = false;
            CameraCollision();
            SamePosition();
            CameraRotation();
        } else
        {
            AwakeningCameraPost();
        }
    }
}
