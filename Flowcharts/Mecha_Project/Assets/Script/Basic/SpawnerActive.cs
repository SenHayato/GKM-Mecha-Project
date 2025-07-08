using System.Collections;
using UnityEngine;

public class SpawnerActive : MonoBehaviour
{
    [SerializeField] private float spawnerDuration;
    [SerializeField] private int maxEnemyInArea; // Spawn musuh jika kurang dari ....
    [SerializeField] private GameObject[] spawnPrefabs;
    [SerializeField] private Transform[] spawnerPosition;
    [SerializeField] private bool spawnerReady = true;
    [SerializeField] GameMaster gameMaster;

    //flag

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

            int rate = Random.Range(0, 100);
            int currentEnemyCount = FindObjectsOfType<EnemyModel>().Length;
            if (currentEnemyCount < maxEnemyInArea) //awal 20
            {
                if (rate < 60) //1-64
                {
                    int prefabNum = Random.Range(0, 2); //0,1
                    Instantiate(spawnPrefabs[prefabNum], spawnerPosition[prefabNum].position, Quaternion.identity);
                }
                else
                {
                    int prefabNum = Random.Range(2, 4); //2,3
                    Instantiate(spawnPrefabs[prefabNum], spawnerPosition[prefabNum].position, Quaternion.identity);
                }
            }
        }
    }

    void SpawnerMaxEnemy()
    {
        if (gameMaster.StageType == StageType.Stage2)
        {
            if (gameMaster.timer <= 30) maxEnemyInArea = 30;
            else if (gameMaster.timer <= 60) maxEnemyInArea = 20;
            else if (gameMaster.timer <= 120) maxEnemyInArea = 10;
        }
    }

    bool gameFinish = false;
    private EnemyModel[] enemyModels;

    void EnemyAllDeath()
    {
        if (gameMaster.timer <= 0.1f)
        {
            spawnerReady = false;
        }
        else
        {
            spawnerReady = true;
        }

        if (gameMaster.timer <= 0f)
        {
            enemyModels = FindObjectsOfType<EnemyModel>();
            if (!gameFinish)
            {
                gameFinish = true;

                foreach(var enemy in enemyModels)
                {
                    enemy.isDeath = true;
                }
            }
        }
    }

    private void Update()
    {
        SpawnerMaxEnemy();
        EnemyAllDeath();
    }
}
