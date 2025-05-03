using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UltimateScript : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;
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

    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah layer dari objek 'other' termasuk dalam enemyLayerMask
        if ((playerActive.enemyLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent<EnemyActive>(out var enemy))
            {
                StartCoroutine(ApplyDamageOverTime(enemy));
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
}
