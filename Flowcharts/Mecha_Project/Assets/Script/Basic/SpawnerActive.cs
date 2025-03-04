using System.Collections;
using UnityEngine;

public class SpawnerActive : MonoBehaviour
{
    [SerializeField] private float spawnerDuration;
    [SerializeField] private int maxEnemyInArea; // Spawn musuh jika kurang dari ....
    [SerializeField] private GameObject[] spawnPrefabs;
    [SerializeField] private bool spawnerReady = true;

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
            if (currentEnemyCount < maxEnemyInArea)
            {
                int spawnerNumber = Random.Range(0, spawnPrefabs.Length);
                Instantiate(spawnPrefabs[spawnerNumber], transform.position, Quaternion.identity);
                Debug.Log("Banyak Musuh " + (currentEnemyCount + 1));
            }

            if (currentEnemyCount == maxEnemyInArea)
            {
                Debug.Log("Musuh sudah mencapai batas maksimal");
            }
        }
    }
}
