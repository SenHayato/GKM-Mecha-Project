using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill2Script : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;

    private Collider[] enemyCollider;

    [SerializeField] Vector3 boxSize;
    [SerializeField] GameObject slashHitEffect;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
        playerActive = GetComponentInParent<PlayerActive>();
    }

    private void OnEnable()
    {
        //buat collider manual dengan overlapbox
        enemyCollider = Physics.OverlapBox(transform.position, boxSize / 2f, transform.rotation, playerActive.enemyLayer);

        foreach (var hitCollider in enemyCollider)
        {
            Vector3 hitPosition = hitCollider.transform.position;
            hitPosition.y = 1.2f;
            Instantiate(slashHitEffect, hitPosition, Quaternion.identity);

            if (hitCollider.TryGetComponent<EnemyActive>(out var enemy))
            {
                enemy.TakeDamage(playerData.skill2Damage);
            }
        }
    }

    private void OnDisable()
    {
        enemyCollider = null;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisasi area box
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
