using Microsoft.Win32.SafeHandles;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
    [SerializeField] MechaPlayer Mecha;
    InputAction lookAction;

    [Header("ScopeCamera")]
    public float defaultFOV;
    public float scopeFOV;
    public float boostFOV;

    public float rotationSpeed;
    Vector2 lookInput;
    public Quaternion defaultCamRot;
    
    //Flag
    private Vector3 currentRecoil;
    private float currentLerpTime = 0f;
    private bool isAiming = false;

    private void Awake()
    {
        weaponSkrip = FindObjectOfType<WeaponRaycast>();
        Player = GameObject.FindGameObjectWithTag("Player");
        Mecha = FindAnyObjectByType<MechaPlayer>();
        cameraControl = FindAnyObjectByType<PlayerInput>();
        PlayerAct = Player.GetComponent<PlayerActive>();
    }
    private void Start()
    {
        MainCamera = MainCameraOBJ.GetComponentInChildren<Camera>();
        lookAction = cameraControl.actions.FindAction("Look");
        MainCamera.transform.position = cameraMainPost.transform.position;
    }

    public void CameraCollision()
    {
        Vector3 direction = Player.transform.position - MainCamera.transform.position;
        float distance = direction.magnitude;
        Debug.DrawRay(Player.transform.position, direction, Color.yellow);

        if (Physics.Raycast(MainCamera.transform.position, direction, out RaycastHit hit, distance, collisionLayers))
        {
            Debug.Log("Collision Aktif");
            cameraMainPost.transform.position = hit.point + new Vector3(0f, 0f, 0f);
        }
        else
        {
            Debug.Log("Collision Deaktif");
            cameraMainPost.transform.position = defaultMainPost.transform.position;
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
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime * recoilSpeed;
            Quaternion targetRotation = Quaternion.Euler(-currentRecoil.y, currentRecoil.x, 0f);
            MainCameraOBJ.transform.localRotation = Quaternion.Lerp(MainCameraOBJ.transform.localRotation, targetRotation, elapsedTime / 0.1f);
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

    private void Update()
    {
        //CameraCollision();
        SamePosition();
        CameraRotation();
    }
}
