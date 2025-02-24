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

    [Header("Weapon Atribut")]
    [Range(0f, 10f)] public float recoilValue;
    [Range(0f, 10f)] public float recoilSpeed;
    public float fireRate;
    public float reloadSpeed;
    public float range = 100f;
    public float bulletSpeed = 200f;
    public bool canShoot = true;
    public bool readytoShoot = true;
    public int ammo;
    public int maxAmmo;
    private bool isReloading = false;

    private void Awake()
    {
        player = GetComponentInParent<MechaPlayer>();
        playerSkrip = GetComponentInParent<PlayerActive>();
        mainCamera = Camera.main;
    }
    void Start()
    {
        cameraAct = mainCamera.GetComponentInParent<CameraActive>();
        ammo = maxAmmo;
        lineRenderer.enabled = false;
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
                }
            }

            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Debug.Log("Tembakan kena target!");
            }
        }
        else
        {
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
        if (ammo == maxAmmo || isReloading || player.isDashing) yield break;

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
}
