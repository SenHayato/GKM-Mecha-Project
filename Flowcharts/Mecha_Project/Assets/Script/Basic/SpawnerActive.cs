using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnerActive : MonoBehaviour
{
    [SerializeField] private float spawnerDuration;
    [SerializeField] private int maxEnemyInArea; // Spawn musuh jika kurang dari ....
    [SerializeField] private GameObject[] spawnPrefabs;
    [SerializeField] private bool spawnerReady = true;
    [SerializeField] GameMaster gameMaster;


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
            if (gameMaster.timer <= 120f) //waktu default 3 menit
            {
                maxEnemyInArea = 20;
            }

            if (gameMaster.timer <= 60f)
            {
                maxEnemyInArea = 25;
            }

            if (gameMaster.timer <= 30f)
            {
                maxEnemyInArea = 35;
            }
        }
    }

    private void Update()
    {
        SpawnerMaxEnemy();
    }
}
