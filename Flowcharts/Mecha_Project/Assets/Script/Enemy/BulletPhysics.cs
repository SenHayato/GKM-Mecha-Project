using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] float bulletSpeed;
    [SerializeField] GameObject hitEffect;

    private int bulletDamage;

    // Start is called before the first frame update
    void Start()
    {
        bulletDamage = enemyModel.attackPower;
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(bulletDamage);
            }
        }
        else
        {
            bulletDamage = 0;
        }

        Instantiate(hitEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += bulletSpeed * Time.deltaTime * transform.forward;
    }
}
