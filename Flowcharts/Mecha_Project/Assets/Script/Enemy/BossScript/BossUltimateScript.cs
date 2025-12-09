using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossUltimateScript : MonoBehaviour
{
    [SerializeField] int ultimateDamage;
    [SerializeField] float ultimateInterval;
    [SerializeField] GameObject hitEffect;

    private bool isDamaging = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var playerActive) && !isDamaging)
            {
                StartCoroutine(GiveDamage(playerActive));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isDamaging = false;
        }
    }

    IEnumerator GiveDamage(PlayerActive playerAct)
    {
        isDamaging = true;

        while (isDamaging)
        {
            Vector3 hitPost = playerAct.transform.position;
            hitPost.y = 1.2f;
            Instantiate(hitEffect, hitPost, Quaternion.identity);
            playerAct.TakeDamage(ultimateDamage);
            yield return new WaitForSeconds(ultimateInterval);
        }
    }
}
