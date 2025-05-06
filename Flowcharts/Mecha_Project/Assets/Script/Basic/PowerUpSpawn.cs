using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpSpawn : MonoBehaviour
{
    [SerializeField] float spawnerDuration;
    [SerializeField] float timerToSpawn;
    [SerializeField] bool isReady;
    [SerializeField] GameObject[] powerUpPrefabs;

    //flag
    //bool isSpawning = false;
    void Start()
    {
        timerToSpawn = spawnerDuration;
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
        if (other.CompareTag("Player"))
        {
            isReady = true;
        }
    }

    void SpawnPowerUp()
    {
        if (isReady)
        {
            timerToSpawn -= Time.deltaTime;

            if (timerToSpawn <= 0)
            {
                isReady = false;
                int powerNumber = Random.Range(0, powerUpPrefabs.Length);
                Instantiate(powerUpPrefabs[powerNumber], transform.position, Quaternion.identity);
                Debug.Log("Spawn Power Up Nomor " + (powerNumber + 1));
                timerToSpawn = spawnerDuration;
            }
        }
        else
        {
            timerToSpawn = spawnerDuration;
        }
    }

    void Update()
    {
        SpawnPowerUp();
    }
}
