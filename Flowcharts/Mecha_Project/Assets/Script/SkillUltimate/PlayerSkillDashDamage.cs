using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillDashDamage : MonoBehaviour
{
    [SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] PlayerActive playerActive;
    [SerializeField] int damageValue;
    [SerializeField] Vector3 boxSize;
    [SerializeField] GameObject hitSlashEffect;

    private Collider[] enemyCollider;
    // Start is called before the first frame update
    void Start()
    {
        damageValue += mechaPlayer.AttackPow + mechaPlayer.skill1Damage;
    }

    void GiveDamage()
    {
        enemyCollider = Physics.OverlapBox(transform.position, boxSize / 2f, transform.rotation, playerActive.enemyLayer);

        foreach (var hitCollider in enemyCollider)
        {
            Vector3 hitPosition = hitCollider.transform.position;
            hitPosition.y = 1.2f;
            Instantiate(hitSlashEffect, hitPosition, Quaternion.identity);

            if (hitCollider.TryGetComponent<EnemyActive>(out var enemy))
            {
                enemy.TakeDamage(damageValue);
            }
        }
    }

    private void FixedUpdate()
    {
        GiveDamage();
    }

    private void OnDisable()
    {
        enemyCollider = null;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisasi area box
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
