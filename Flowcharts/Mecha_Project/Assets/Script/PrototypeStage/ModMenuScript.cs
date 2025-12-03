using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModMenuScript : MonoBehaviour
{
    [SerializeField] PlayerActive playerActive;
    [SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] EnemyModel[] enemyModels;

    [Header("Enemy PreFabs")]
    [SerializeField] GameObject bossPrefab;
    [SerializeField] GameObject meleeEnemy;
    [SerializeField] GameObject rangeEnemy;
    [SerializeField] GameObject eliteMelee;
    [SerializeField] GameObject eliteRange;

    [Header("PowerUp Prefabs")]
    [SerializeField] GameObject attackPowerUp;
    [SerializeField] GameObject defendPowerUp;
    [SerializeField] GameObject energyPowerUp;
    [SerializeField] GameObject ultimatePowerUp;
    [SerializeField] GameObject healthPowerUp;

    [Header("Spawner Position")]
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] Transform[] powerSpawnPost;
    void Awake()
    {
        playerActive = FindFirstObjectByType<PlayerActive>();
        mechaPlayer = FindFirstObjectByType<MechaPlayer>();
    }

    void Start()
    {
        
    }


    private void Update()
    {
        GetEnemy();
        EnemyNav();
        Debug.Log("GetEnemy");
    }

    void GetEnemy()
    {
        enemyModels = FindObjectsOfType<EnemyModel>();
    }

    NavMeshAgent[] enemyNavAgent;
    void EnemyNav()
    {
        enemyNavAgent = FindObjectsOfType<NavMeshAgent>();
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

    public void PlayerFullMax()
    {
        mechaPlayer.Health = mechaPlayer.MaxHealth;
    }

    public void PlayerFullUlt()
    {
        mechaPlayer.Ultimate = mechaPlayer.MaxUltimate;
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
        if (enemyNavAgent != null)
        {
            foreach (var agent in enemyNavAgent)
            {
                foreach (var model in enemyModels)
                {
                    agent.speed = model.defaultSpeed;
                }
            }
        }
    }

    public void KillAllEnemy()
    {
        if (enemyModels != null)
        {
            foreach (var model in enemyModels)
            {
                model.health = 0;
            }
        }
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
                powerUpToSpawn = attackPowerUp;
                break;
            case 2:
                powerUpToSpawn = defendPowerUp;
                break;
            case 3:
                powerUpToSpawn = energyPowerUp;
                break;
            case 4:
                powerUpToSpawn = ultimatePowerUp;
                break;
            case 5:
                powerUpToSpawn = healthPowerUp;
                break;
        }

        Instantiate(powerUpToSpawn, powerSpawnPost[powerNumber - 1].position, powerSpawnPost[powerNumber - 1].rotation);
        Debug.Log(powerUpToSpawn.name + " spawn");
    }
    #endregion
}
