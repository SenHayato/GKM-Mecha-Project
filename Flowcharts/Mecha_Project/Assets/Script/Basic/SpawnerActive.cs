using System.Collections;
using UnityEngine;

public class SpawnerActive : MonoBehaviour
{
    [SerializeField] private float spawnerDuration;
    [SerializeField] private int maxEnemyInArea; // Spawn musuh jika kurang dari ....
    [SerializeField] private GameObject[] spawnPrefabs;
    [SerializeField] private bool spawnerReady = true;
    [SerializeField] GameMaster gameMaster;

    //flag
    //private int MaxState1;
    //private int MaxState2;
    //private int MaxState3;

    private void Awake()
    {
        gameMaster = FindFirstObjectByType<GameMaster>();
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        while (spawnerReady)
        {
            yield return new WaitForSeconds(spawnerDuration);

            int currentEnemyCount = FindObjectsOfType<EnemyModel>().Length;
            if (currentEnemyCount < maxEnemyInArea) //awal 20
            {
                int spawnerNumber = Random.Range(0, spawnPrefabs.Length);
                Instantiate(spawnPrefabs[spawnerNumber], transform.position, Quaternion.identity);
                Debug.Log("Banyak Musuh " + (currentEnemyCount + 1));
            }
        }
    }

    void SpawnerMaxEnemy()
    {
        Debug.Log("MaxEnemySpawn menjadi " + maxEnemyInArea);
        if (gameMaster.StageType == StageType.Stage2)
        {
            if (gameMaster.timer <= 30) maxEnemyInArea = 30;
            else if (gameMaster.timer <= 60) maxEnemyInArea = 20;
            else if (gameMaster.timer <= 120) maxEnemyInArea = 15;
        }
    }

    private void Update()
    {
        SpawnerMaxEnemy();
    }
}
