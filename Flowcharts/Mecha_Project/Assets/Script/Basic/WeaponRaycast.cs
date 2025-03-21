using UnityEngine;
using System.Collections;
using UnityEditor.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;

public class WeaponRaycast : MonoBehaviour
{
    public LayerMask hitLayers;
    public GameObject hitEffect;
    public Transform bulletSpawn;
    public MechaPlayer mechaPlayer;
    public PlayerActive playerSkrip;
    public Camera mainCamera;
    public LineRenderer lineRenderer;
    public CameraActive cameraAct;
    public HUDGameManager HUDManager;
    HashSet<string> enemyTags;

    [Header("Weapon Atribut")]
    [Range(0f, 10f)] public float recoilValueX;
    [Range(0f, 10f)] public float recoilValueY;
    [Range(0f, 10f)] public float recoilSpeed;
    public AudioSource audioSource;
    public float fireRate;
    public float reloadSpeed;
    public float range = 100f;
    //public float bulletSpeed = 200f;
    public bool canShoot = true;
    public bool readytoShoot = true;
    public int ammo;
    public int maxAmmo;

    [Header("Sound Library")]
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip rechargeSound; //Reload SFX
    [SerializeField] bool fireActive = false;
    [SerializeField] bool rechargeActive = false;

    //flag
    private bool isReloading = false;
    private float defaultValueX; //default recoil x
    private float defaultValueY; //default recoil y

    private void Awake()
    {
        mechaPlayer = GetComponentInParent<MechaPlayer>();
        playerSkrip = GetComponentInParent<PlayerActive>();
        mainCamera = Camera.main;
        HUDManager = FindAnyObjectByType<HUDGameManager>();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cameraAct = mainCamera.GetComponentInParent<CameraActive>();
        ammo = maxAmmo;
        lineRenderer.enabled = false;
        defaultValueX = recoilValueX;
        defaultValueY = recoilValueY;
        enemyTags = playerSkrip.enemyTags;
    }

    void SoundMonitor()
    {
        if (mechaPlayer.isShooting)
        {
            audioSource.clip = fireSound;
            audioSource.loop = true;
            if (!fireActive)
            {
                fireActive = true;
                audioSource.Play();
            }
        }
        else
        {
            fireActive = false;
            audioSource.loop = false;
        }

        if (mechaPlayer.isReloading)
        {
            audioSource.clip = rechargeSound;
            audioSource.loop = false;
            if (!rechargeActive)
            {
                rechargeActive = true;
                audioSource.Play();
            }
        }
        else
        {
            rechargeActive = false;
        }
    }

    public void AwakeningMonitor()
    {
        mechaPlayer.Awakening = Mathf.Clamp(mechaPlayer.Awakening, mechaPlayer.MinAwakening, mechaPlayer.MaxAwakening);
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

            if (enemyTags.Contains(hit.collider.tag))
            {
                if (hit.collider.TryGetComponent<EnemyActive>(out var enemy))
                {
                    enemy.TakeDamage(mechaPlayer.AttackPow);
                    if (!mechaPlayer.UsingAwakening)
                    {
                        mechaPlayer.Awakening += mechaPlayer.AwakeningRegen;
                    }
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
        if (isReloading) yield break;

        ammo = 0;
        isReloading = true;
        mechaPlayer.isDashing = false;
        playerSkrip.speed = mechaPlayer.defaultSpeed;
        mechaPlayer.isReloading = isReloading;
        readytoShoot = false;

        Debug.Log("Reloading");
        yield return new WaitForSecondsRealtime(reloadSpeed);

        ammo = maxAmmo;
        isReloading = false;
        mechaPlayer.isReloading = isReloading;
        readytoShoot = true;
    }

    public void RecoilAdjust()
    {
        if (mechaPlayer.isAiming)
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
        SoundMonitor();
        RecoilAdjust();
        AwakeningMonitor();
    }
}
