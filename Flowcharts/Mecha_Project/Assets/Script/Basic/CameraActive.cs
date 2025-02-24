using Microsoft.Win32.SafeHandles;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraActive : MonoBehaviour
{
    public GameObject Player;
    public GameObject Camera;
    public WeaponRaycast weaponSkrip;

    public Transform cameraPivot;
    public Transform cameraAimPost;
    public Transform cameraMainPost;
    public PlayerInput cameraControl;
    public PlayerActive PlayerAct;
    public Camera MainCamera;
    private MechaPlayer Mecha;
    InputAction lookAction;

    [Header("ScopeCamera")]
    public float defaultFOV;
    public float scopeFOV;

    public float rotationSpeed;
    Vector2 lookInput;
    public Quaternion defaultCamRot;

    //Flag
    private Vector2 currentRecoil;
    private float currentLerpTime = 0f;
    private bool isAiming = false;

    private void Awake()
    {
        weaponSkrip = FindObjectOfType<WeaponRaycast>();
        Player = GameObject.FindGameObjectWithTag("Player");
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        Mecha = FindAnyObjectByType<MechaPlayer>();
        cameraControl = FindAnyObjectByType<PlayerInput>();
        cameraPivot = GetComponent<Transform>();
        PlayerAct = Player.GetComponent<PlayerActive>();
    }
    private void Start()
    {
        MainCamera = Camera.GetComponent<Camera>();
        lookAction = cameraControl.actions.FindAction("Look");
        //Default camera rotation
        cameraPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        MainCamera.transform.position = cameraMainPost.transform.position;
    }

    public void ScopeCamera()
    {
        float lerpDuration = 0.05f;
        if (Mecha.isAiming)
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

    public void ApplyRecoil()
    {
        StartCoroutine(RecoilEffect());
    }

    public IEnumerator RecoilEffect()
    {
        float recoilUp = weaponSkrip.recoilValue; // Recoil selalu naik
        float recoilSide = Random.Range(-weaponSkrip.recoilValue * 2f, weaponSkrip.recoilValue * 2f); // Acak ke samping
        float recoilSpeed = weaponSkrip.recoilSpeed;
        float elapsedTime = 0f;

        // Tambahkan recoil baru ke recoil yang sedang berlangsung
        currentRecoil += new Vector2(recoilSide, recoilUp);

        // Recoil naik
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime * recoilSpeed;
            Quaternion targetRotation = Quaternion.Euler(-currentRecoil.y, currentRecoil.x, 0f);
            MainCamera.transform.localRotation = Quaternion.Lerp(MainCamera.transform.localRotation, targetRotation, elapsedTime / 0.1f);
            yield return null;
        }

        // Waktu turun (bisa dikurangi agar terasa lebih tajam)
        yield return new WaitForSeconds(1f);

        elapsedTime = 0f;

        // Recoil turun sedikit (biar efeknya tetap ada, tapi tidak langsung ke posisi awal)
        while (weaponSkrip.ammo <= 0 || !Mecha.isShooting)
        {
            elapsedTime += Time.deltaTime * recoilSpeed;
            currentRecoil = Vector2.Lerp(currentRecoil, Vector2.zero, elapsedTime / 0.1f);
            Quaternion targetRotation = Quaternion.Euler(-currentRecoil.y, currentRecoil.x, 0f);
            MainCamera.transform.localRotation = targetRotation;
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
        Player.transform.rotation = Camera.transform.rotation;
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
        SamePosition();
        CameraRotation();
    }
}
