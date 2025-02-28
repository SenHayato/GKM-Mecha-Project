using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UltimateScript : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    private float duration;
    private float interval;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
    }

    private void Start()
    {
        duration = playerData.UltDuration;
        interval = playerData.UltInterval;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(ApplyDamageOverTime(other.GetComponent<EnemyActive>()));
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
