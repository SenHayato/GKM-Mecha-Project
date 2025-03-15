using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UltimateScript : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;
    HashSet<string> enemyTags;
    private float duration;
    private float interval;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
        playerActive = GetComponentInParent<PlayerActive>();
    }

    private void Start()
    {
        enemyTags = playerActive.enemyTags;
        duration = playerData.UltDuration;
        interval = playerData.UltInterval;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyTags.Contains(other.tag))
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
