using UnityEngine;

public class ShortWeapon : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] int additionalAttackDMG;
    [SerializeField] Transform enemyPost;

    [Header("Visual Effect")]
    [SerializeField] GameObject hitSlashEffect;
    [SerializeField] GameObject clawSlashEffect;

    private void Awake()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
    }

    private void OnEnable()
    {
        Instantiate(clawSlashEffect, transform.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPosition = other.transform.position;
        hitPosition.y = 1.2f;
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var player))
            {
                if (!player.Mecha.isBlocking)
                {
                    Instantiate(hitSlashEffect, hitPosition, Quaternion.identity);
                    player.TakeDamage(enemyModel.attackPower + additionalAttackDMG);
                    Debug.Log("SSS");
                }
            }
        }
    }
}
