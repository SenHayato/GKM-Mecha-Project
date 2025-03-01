using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDmg : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(5000);
            }
        }
    }
}
