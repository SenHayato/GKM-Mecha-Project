using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpSpawn : MonoBehaviour
{
    [SerializeField] float spawnerDuration;
    [SerializeField] float timerToSpawn;
    [SerializeField] bool isReUse;
    [SerializeField] bool isReady;
    [SerializeField] GameObject[] powerUpPrefabs;
    void Start()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            isReady = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            isReady = true;
        }
    }

    void SpawnPowerUp()
    {
        if (isReady)
        {
            timerToSpawn -= 1 * Time.deltaTime;

        }
    }
    void Update()
    {
        SpawnPowerUp();
    }
}
