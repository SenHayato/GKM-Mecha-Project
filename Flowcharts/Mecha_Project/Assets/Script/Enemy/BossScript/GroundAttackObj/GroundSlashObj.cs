using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlashObj : MonoBehaviour
{
    //Object akan diduplicate bukan di instantiate
    [Header("Referensi")]
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] float destroyTime;

    [Header("Ground Slash Attribut")]
    [SerializeField] int damageValue;


    // Start is called before the first frame update
    void Start()
    {
        damageValue += enemyModel.attackPower;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(damageValue);
            }
        }
    }

    void MoveForward()
    {
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * transform.forward);
    }
        
    private void Update()
    {
        MoveForward();
        Destroy(gameObject, destroyTime);
    }
}
