using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class UltimateScript : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;
    [SerializeField] Vector3 boxSize;
    [SerializeField] GameObject hitEffect;

    private Collider[] enemyColliders;

    private bool giveDamage = true;
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

    private void Update()
    {
        FindEnemy();
        //GiveDamage();
    }

    void FindEnemy()
    {
        if (giveDamage)
        {
            giveDamage = false;
            enemyColliders = Physics.OverlapBox(transform.position, boxSize / 2f, transform.rotation, playerActive.enemyLayer);
            GiveDamage();
            Invoke(nameof(ResetDamage), interval);
        }
    }

    void GiveDamage()
    {
        if (enemyColliders != null)
        {
            foreach (var hitCollider in enemyColliders)
            {
                Vector3 hitPosition = hitCollider.transform.position;
                hitPosition.y = 1.2f;
                Instantiate(hitEffect, hitPosition, Quaternion.identity);

                if (hitCollider.TryGetComponent<EnemyActive>(out var enemyActive))
                {
                    enemyActive.enemyModel.isStunt = true;
                    enemyActive.TakeDamage(playerData.UltDamage);
                }
            }
        }
    }

    void ResetDamage()
    {
        giveDamage = true;
    }

    private void OnDisable()
    {
        giveDamage = true;

        if (enemyColliders != null)
        {
            foreach (var hitCollider in enemyColliders)
            {
                if (hitCollider != null && hitCollider.TryGetComponent<EnemyActive>(out var enemyActive))
                {
                    enemyActive.enemyModel.isStunt = false;
                }
            }

            enemyColliders = null;
        }
    }


    private void OnDrawGizmosSelected()
    {
        // Visualisasi area
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
