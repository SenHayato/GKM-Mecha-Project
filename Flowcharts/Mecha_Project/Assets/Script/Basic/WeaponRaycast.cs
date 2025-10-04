using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;

public class WeaponRaycast : MonoBehaviour
{
    [Header("Referensi")]
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
    [SerializeField] Transform pointToShoot;
    [SerializeField] Ray ray;
    [Range(0f, 10f)] public float recoilValueX;
    [Range(0f, 10f)] public float recoilValueY;
    [Range(0f, 10f)] public float recoilSpeed;
    public AudioSource audioSource;
    [SerializeField] int weaponDamage;
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

    [Header("Visual Effect")]
    [SerializeField] VisualEffect fireBulletVFX;
    [SerializeField] Animation anim;

    //flag
    private bool isReloading = false;
    private float defaultValueX; //default recoil x
    private float defaultValueY; //default recoil y

    private void Awake()
    {
        pointToShoot = GameObject.FindGameObjectWithTag("ShootPoint").transform;
        mechaPlayer = GetComponentInParent<MechaPlayer>();
        playerSkrip = GetComponentInParent<PlayerActive>();
        mainCamera = Camera.main;
        HUDManager = FindAnyObjectByType<HUDGameManager>();
    }

    void Start()
    {
        fireBulletVFX.enabled = false;
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

    void RayChange()
    {
        Vector3 screenCenter = new(Screen.width / 2, Screen.height / 2, 0);
        Ray centerRay = mainCamera.ScreenPointToRay(screenCenter);
        Vector3 direction = (centerRay.direction).normalized;

        if (mechaPlayer.isAiming)
        {
            ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }
        else
        {
            ray = new(pointToShoot.position, direction);
        }
    }

    public IEnumerator FireShoot()
    {
        if (!canShoot || isReloading || ammo <= 0) yield break;

        canShoot = false;
        ammo--;
        StartCoroutine(cameraAct.RecoilEffect());
        Vector3 targetPoint;
        mechaPlayer.skill2Bar++;
        //Debug.DrawRay(pointToShoot.position, pointToShoot.forward, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
        {
            targetPoint = hit.point;
            if (enemyTags.Contains(hit.collider.tag))
            {
                mechaPlayer.Ultimate += mechaPlayer.UltRegenValue;
                if (hit.collider.TryGetComponent<EnemyActive>(out var enemy))
                {
                    enemy.TakeDamage(mechaPlayer.AttackPow + weaponDamage);
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
        if (isReloading) yield break;

        ammo = 0;
        isReloading = true;
        mechaPlayer.isDashing = false;
        playerSkrip.speed = mechaPlayer.defaultSpeed;
        mechaPlayer.isReloading = isReloading;
        readytoShoot = false;

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

    void VisualEffectSet()
    {
        fireBulletVFX.SetFloat("SpawnRate_", fireRate);
        if (mechaPlayer.isShooting)
        {
            fireBulletVFX.enabled = true;
        }
        else
        {
            fireBulletVFX.enabled = false;
        }
    }

    private bool isCoolingDown = true;
    void BladeFlammingRed()
    {
        if (mechaPlayer.skill2Bar >= 8 && isCoolingDown)
        {
            isCoolingDown = false;
            anim.Play("BladeHot");
        }

        if (mechaPlayer.usingSkill2 && !isCoolingDown)
        {
            isCoolingDown = true;
            anim.Play("BladeCold");
        }
    }

    private void Update()
    {
        VisualEffectSet();
        RayChange();
        SoundMonitor();
        RecoilAdjust();
        AwakeningMonitor();
        BladeFlammingRed();
    }
}
