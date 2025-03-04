using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerActive : MonoBehaviour
{
    [SerializeField] float spawnerDuration;
    [SerializeField] int maxEnemyInArea; //Maksimal musuh di area
    [SerializeField] GameObject[] spawnPrefabs;
    [SerializeField] bool spawnerReady;

    //checker
    EnemyModel[] enemies;
    int spawnerNumber;

    private void Start()
    {
        spawnerReady = false;
    }

    public void EnemyChecker()
    {
        enemies = FindObjectsOfType<EnemyModel>();
        spawnerNumber = Random.Range(0, spawnPrefabs.Length);
    }

    private void SpawnEnemy()
    {
        if (enemies.Length <= maxEnemyInArea)
        {
            spawnerReady = true;
        }
        
        if (enemies.Length > maxEnemyInArea)
        {
            spawnerReady = false;
            Debug.Log("Enemy melebihi batas");
        }
    }

    private IEnumerator EnemySpawning()
    {
        if (spawnerReady)
        {
            Instantiate(spawnPrefabs[spawnerNumber], this.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnerDuration);
            spawnerReady = false;
        }
    }

    void Update()
    {
        StartCoroutine(EnemySpawning());
        EnemyChecker();
        SpawnEnemy();
    }
}
