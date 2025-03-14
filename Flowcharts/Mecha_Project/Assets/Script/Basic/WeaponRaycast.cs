using UnityEngine;
using System.Collections;
using UnityEditor.UI;

public class WeaponRaycast : MonoBehaviour
{
    public LayerMask hitLayers;
    public GameObject hitEffect;
    public Transform bulletSpawn;
    public MechaPlayer player;
    public PlayerActive playerSkrip;
    public Camera mainCamera;
    public LineRenderer lineRenderer;
    public CameraActive cameraAct;
    public HUDGameManager HUDManager;

    [Header("Weapon Atribut")]
    [Range(0f, 10f)] public float recoilValueX;
    [Range(0f, 10f)] public float recoilValueY;
    [Range(0f, 10f)] public float recoilSpeed;
    public float fireRate;
    public float reloadSpeed;
    public float range = 100f;
    public float bulletSpeed = 200f;
    public bool canShoot = true;
    public bool readytoShoot = true;
    public int ammo;
    public int maxAmmo;

    //flag
    private bool isReloading = false;
    private float defaultValueX; //default recoil x
    private float defaultValueY; //default recoil y

    private void Awake()
    {
        player = GetComponentInParent<MechaPlayer>();
        playerSkrip = GetComponentInParent<PlayerActive>();
        mainCamera = Camera.main;
        HUDManager = FindAnyObjectByType<HUDGameManager>();
    }
    void Start()
    {
        cameraAct = mainCamera.GetComponentInParent<CameraActive>();
        ammo = maxAmmo;
        lineRenderer.enabled = false;
        defaultValueX = recoilValueX;
        defaultValueY = recoilValueY;
    }
    public IEnumerator FireShoot()
    {
        if (!canShoot || isReloading || ammo <= 0) yield break;

        canShoot = false;
        ammo--;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        StartCoroutine(cameraAct.RecoilEffect());
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
        {
            targetPoint = hit.point;

            if (hit.collider.CompareTag("Enemy"))
            {
                if (hit.collider.TryGetComponent<EnemyActive>(out var enemy))
                {
                    enemy.TakeDamage(player.AttackPow);
                    HUDManager.hitEffect.color = Color.red;
                    yield return new WaitForSeconds(fireRate);
                    HUDManager.hitEffect.color = Color.white;
                }
            }
            else
            {
                HUDManager.hitEffect.color = Color.white;
            }

            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                HUDManager.hitEffect.color = Color.white;
                Debug.Log("Tembakan kena target!");
            }
        }
        else
        {
            HUDManager.hitEffect.color = Color.white;
            targetPoint = ray.GetPoint(range);
        }

        StartCoroutine(BulletTrailEffect(targetPoint));
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
    private IEnumerator BulletTrailEffect(Vector3 targetPoint)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, bulletSpawn.transform.position);
        lineRenderer.SetPosition(1, targetPoint);  
        yield return new WaitForSeconds(fireRate * 1.5f);
        lineRenderer.enabled = false;
    }
    public IEnumerator ReloadAmmo()
    {
        if (isReloading || player.isDashing) yield break;

        ammo = 0;
        isReloading = true;
        player.isReloading = isReloading;
        readytoShoot = false;

        Debug.Log("Reloading");
        yield return new WaitForSecondsRealtime(3.30f);

        ammo = maxAmmo;
        isReloading = false;
        player.isReloading = isReloading;
        readytoShoot = true;
    }

    public void RecoilAdjust()
    {
        if (player.isAiming)
        {
            recoilValueX = 0.5f;
            recoilValueY = 0.15f;
        }
        else
        {
            recoilValueX = defaultValueX;
            recoilValueY = defaultValueY;
        }
    }

    private void Update()
    {
        RecoilAdjust();
    }
}
