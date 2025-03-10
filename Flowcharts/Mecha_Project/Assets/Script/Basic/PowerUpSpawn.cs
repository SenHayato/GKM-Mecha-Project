using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpSpawn : MonoBehaviour
{
    [SerializeField] float spawnerDuration;
    [SerializeField] float timerToSpawn;
    [SerializeField] bool isReady;
    [SerializeField] GameObject[] powerUpPrefabs;
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
        if (other.CompareTag("PowerUp"))
        {
            isReady = true;
        }
    }

    void SpawnPowerUp()
    {
        if (isReady)
        {
            int powerNumber = Random.Range(0, powerUpPrefabs.Length);
            timerToSpawn -= 1 * Time.deltaTime;
            if (timerToSpawn <= 0)
            {
                timerToSpawn = 0;
                Instantiate(powerUpPrefabs[powerNumber], transform.position, Quaternion.identity);
                Debug.Log("Spawn Power Up Nomor" + powerNumber);
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
