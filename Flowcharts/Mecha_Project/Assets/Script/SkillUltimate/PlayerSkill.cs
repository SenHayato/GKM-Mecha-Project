using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] int DamageValue;
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;
    [SerializeField] float skillDuration;

    [SerializeField] float forwardSpeed;

    private Collider[] enemyCollider;

    [SerializeField] Vector3 boxSize;

    private void Start()
    {
        DamageValue += playerData.AttackPow + playerData.skill1Damage;
    }

    void GiveDamage()
    {
        enemyCollider = Physics.OverlapBox(transform.position, boxSize / 2f, transform.rotation, playerActive.enemyLayer);
        
        foreach (var hitCollider in enemyCollider)
        {
            if (hitCollider.TryGetComponent<EnemyActive>(out var enemy))
            {
                enemy.TakeDamage(DamageValue);
            }
        }
    }

    private void FixedUpdate()
    {
        GiveDamage();
        transform.position += forwardSpeed * Time.deltaTime * transform.forward;
        Destroy(gameObject, skillDuration);
    }

    private void OnDestroy()
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
