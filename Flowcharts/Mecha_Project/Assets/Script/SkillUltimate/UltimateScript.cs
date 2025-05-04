using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UltimateScript : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;
    [SerializeField] float damageRadius;

    private float duration;
    private float interval;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
        playerActive = GetComponentInParent<PlayerActive>();
    }

    private void Start()
    {
        duration = playerData.UltDuration;
        interval = playerData.UltInterval;
    }

    private void OnEnable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius, playerActive.enemyLayer);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<EnemyActive>(out var enemyActive))
            {
                StartCoroutine(ApplyDamageOverTime(enemyActive));
            }
        }

    }

    private IEnumerator ApplyDamageOverTime(EnemyActive enemy)
    {
        for (float t = 0; t < duration; t += interval)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(playerData.UltDamage);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisasi area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
