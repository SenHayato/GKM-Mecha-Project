using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerActive : MonoBehaviour
{
    [SerializeField] float spawnerDuration;
    [SerializeField] int maxEnemyInARea; //Maksimal musuh di area
    [SerializeField] GameObject[] spawnPrefabs;

    //checker
    EnemyModel[] enemies;

    private void SpawnEnemy()
    {
        enemies = FindObjectsOfType<EnemyModel>();
        int spawnerNumber = Random.Range(0, spawnPrefabs.Length);
        Debug.Log(spawnerNumber.ToString());
        //if (enemies.Length < maxEnemyInARea)
        //{
            
        //}
    }

    void Update()
    {
        SpawnEnemy();
    }
}
