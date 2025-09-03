using UnityEngine;

public class Boss360Attack : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] int attackValue;
    [SerializeField] GameObject hitEffect;

    //flag
    int attackDamage;

    private void Awake()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
    }

    private void Start()
    {
        attackDamage = enemyModel.attackPower + attackValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 hitPost = other.transform.position;
            hitPost.y = 1.2f;
            Instantiate(hitEffect, hitPost, Quaternion.identity);

            if (other.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(attackDamage);
            }
        }
    }
}
