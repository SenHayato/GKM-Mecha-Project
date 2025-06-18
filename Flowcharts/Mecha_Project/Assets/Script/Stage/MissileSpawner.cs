using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Transform playerPosition;
    //[SerializeField] GameObject missileObj;
    [SerializeField] float untilSpawn;
    [SerializeField] float spawnDuration;

    [Header("SpawnerStatus")]
    [SerializeField] bool isSpawned = false;

    //flag
    bool wasSpawned = false;

    private void Awake()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        untilSpawn = spawnDuration;
    }

    void TimeDuration()
    {
        if (!wasSpawned)
        {
            spawnDuration -= Time.deltaTime;
        }
        else
        {
            untilSpawn = spawnDuration;
        }

        if (untilSpawn <= 0f)
        {
            isSpawned = true;
        }
    }

    void SpawningMissile()
    {
        if (isSpawned && !wasSpawned)
        {
            isSpawned= false;
            wasSpawned = true;
            Debug.Log("TestMissile");
        }
    }
    
    void Update()
    {
        TimeDuration();
        SpawningMissile();
    }
}
