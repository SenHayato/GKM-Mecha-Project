using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    [Header("Sound Library")]
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip slashSound;
    [SerializeField] AudioClip explodedSound;

    [Header("Sound Trigger")]
    [SerializeField] bool isFiring = false;
    [SerializeField] bool isSlashing = false;
    [SerializeField] bool isExploded = false;

    [Header("Sound Set Up")]
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] AudioSource soundSource;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
        enemyModel = GetComponentInParent<EnemyModel>();
    }

    void EnemyMonitorRange()
    {
        if (enemyModel.enemyType == EnemyType.EnemyRange)
        {
            soundSource.clip = fireSound;
            if (enemyModel.isAttacking && !isFiring)
            {
                isFiring = true;
                soundSource.Play();
            }

            if (!enemyModel.isAttacking)
            {
                isFiring = false;
            }
        }
    }

    void EnemyShortMonitor()
    {
        if (enemyModel.enemyType == EnemyType.EnemyShort)
        {
            soundSource.clip = slashSound;
            if (enemyModel.isAttacking && !isSlashing)
            {
                isSlashing = true;
                soundSource.Play();
            }

            if (!enemyModel.isAttacking)
            {
                isSlashing = false;
            }
        }
    }

    void EnemyExploded()
    {
        if (enemyModel.isDeath && !isExploded)
        {
            soundSource.clip = explodedSound;
            soundSource.Play();
            isExploded = true;
        }
    }

    private void Update()
    {
        EnemyMonitorRange();
        EnemyShortMonitor();
        EnemyExploded();
    }
}
