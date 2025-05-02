using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortWeapon : MonoBehaviour
{
    [SerializeField] EnemyModel model;

    private void Start()
    {
        model = GetComponentInParent<EnemyModel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
           if (other.TryGetComponent<PlayerActive>(out var player)){
                player.TakeDamage(model.attackPower);
                Debug.Log("SSS");
            }
        }
    }
}
