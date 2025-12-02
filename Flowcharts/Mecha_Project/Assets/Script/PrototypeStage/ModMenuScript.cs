using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModMenuScript : MonoBehaviour
{
    [SerializeField] PlayerActive playerActive;
    [SerializeField] EnemyModel[] enemyModels;

    [Header("Enemy PreFabs")]
    [SerializeField] GameObject bossPrefab;
    [SerializeField] GameObject meleeEnemy;
    [SerializeField] GameObject rangeEnemy;
    [SerializeField] GameObject eliteMelee;
    [SerializeField] GameObject eliteRange;

    [Header("Spawner Position")]
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] Transform[] powerSpawnPost;
    void Awake()
    {
        playerActive = FindFirstObjectByType<PlayerActive>();
    }

    void Start()
    {
        
    }


    private void LateUpdate()
    {
        GetEnemy();
        EnemyNav();
    }

    void Update()
    {

    }

    void GetEnemy()
    {
        enemyModels = FindObjectsOfType<EnemyModel>();
        Debug.Log("GetEnemy");
    }

    NavMeshAgent[] enemyNavAgent;
    bool enemyStopped = false;
    void EnemyNav()
    {
        if (enemyModels != null)
        {
            foreach (var model in enemyModels)
            {
                for (int i = 0; i < enemyNavAgent.Length; i++)
                {
                    enemyNavAgent[i] = model.GetComponent<NavMeshAgent>();
                }
            }
        }
    }

    #region PlayerMod --------------------------------------------------------------------
    public void PlayerNullDamage()
    {
        if (playerActive.cheatUndamage)
        {
            playerActive.cheatUndamage = false;
        }
        else
        {
            playerActive.cheatUndamage = true;
        }
    }
    #endregion

    #region EnemyMod ---------------------------------------------------------------------

    GameObject enemyToSpawn;
    public void EnemySpawn(int enemyNumber)
    {
        switch (enemyNumber)
        {
            case 1:
                enemyToSpawn = bossPrefab;
                break;
            case 2:
                enemyToSpawn = meleeEnemy;
                break;
            case 3:
                enemyToSpawn = rangeEnemy;
                break;
            case 4:
                enemyToSpawn = eliteMelee;
                break;
            case 5:
                enemyToSpawn = eliteRange;
                break;
        }

        Instantiate(enemyToSpawn, spawnPositions[enemyNumber - 1].position, spawnPositions[enemyNumber - 1].rotation);
        Debug.Log(enemyToSpawn.name + " spawn");
    }

    public void StartEnemyMove()
    {
        //if (enemyNavAgent != null)
        //{
        //    foreach(var agent in enemyNavAgent)
        //    {
        //        agent.speed = enemyModels;
        //    }
        //}
    }

    public void StopMovingEnemy()
    {
        if (enemyNavAgent != null)
        {
            foreach(var agent in enemyNavAgent)
            {
                agent.speed = 0;
            }
        }
    }
    #endregion

    #region PowerUp ---------------------------------------------------------------------
    GameObject powerUpToSpawn;
    public void PowerUpSpawn(int powerNumber)
    {
        switch (powerNumber)
        {
            case 1:
                powerUpToSpawn = bossPrefab;
                break;
            case 2:
                powerUpToSpawn = meleeEnemy;
                break;
            case 3:
                powerUpToSpawn = rangeEnemy;
                break;
            case 4:
                powerUpToSpawn = eliteMelee;
                break;
            case 5:
                powerUpToSpawn = eliteRange;
                break;
        }

        Instantiate(powerUpToSpawn, spawnPositions[powerNumber - 1].position, spawnPositions[powerNumber - 1].rotation);
        Debug.Log(powerUpToSpawn.name + " spawn");
    }
    #endregion
}
