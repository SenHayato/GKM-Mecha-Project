using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Transform playerPosition;
    [SerializeField] GameObject missileObj;
    [SerializeField] float untilSpawn;
    [SerializeField] float spawnDuration;

    [Header("SpawnerStatus")]
    [SerializeField] bool isSpawned = false;
    [SerializeField] bool spawnerReady = false;

    private void Awake()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        spawnDuration = untilSpawn;
    }

    void TimeDuration()
    {
        if (!spawnerReady)
        {
            isSpawned = false;
            spawnDuration -= 1f * Time.deltaTime;
        }

        if (spawnDuration <= 0f)
        {
            spawnDuration = untilSpawn;
            spawnerReady = true;
            isSpawned = true;
        }
    }

    void SpawningMissile()
    {
        if (isSpawned)
        {
            spawnerReady = false;
            isSpawned = false;
            Instantiate(missileObj, playerPosition.position, Quaternion.identity);
        }
    }
    
    void Update()
    {
        TimeDuration();
        SpawningMissile();
    }
}
