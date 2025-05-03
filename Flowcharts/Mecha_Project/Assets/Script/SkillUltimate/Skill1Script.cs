using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Skill1Script : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;

    [SerializeField] float localScale;
    [SerializeField] float maxDistance;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
        playerActive = GetComponentInParent<PlayerActive>();
    }

    private void Update()
    {
        if (Physics.BoxCast(transform.position, transform.localScale * localScale, transform.forward, out RaycastHit hit, Quaternion.identity, maxDistance, playerActive.enemyLayer))
        {
            Debug.Log("Skill 1 Kena");
            if (hit.collider.TryGetComponent<EnemyActive>(out var enemyActive))
            {
                enemyActive.TakeDamage(playerData.skill1Damage);
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if ((playerActive.enemyLayer.value & (1 << other.gameObject.layer)) != 0)
    //    {
    //        if (other.TryGetComponent<EnemyActive>(out var enemy))
    //        {
    //            enemy.TakeDamage(playerData.skill1Damage);
    //        }
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + transform.forward * maxDistance, transform.localScale);
    }
}
