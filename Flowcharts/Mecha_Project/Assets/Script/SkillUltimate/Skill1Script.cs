using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Skill1Script : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;

    private Collider[] enemyCollider;

    [SerializeField] Vector3 boxSize;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
        playerActive = GetComponentInParent<PlayerActive>();
    }

    private void OnEnable()
    {
        enemyCollider = Physics.OverlapBox(transform.position, boxSize / 2f, transform.rotation, playerActive.enemyLayer);
        
        foreach (var hitCollider in enemyCollider)
        {
            if (hitCollider.TryGetComponent<EnemyActive>(out var enemy))
            {
                enemy.TakeDamage(playerData.skill1Damage);
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
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
