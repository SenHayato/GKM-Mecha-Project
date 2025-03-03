using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurotialCheckPointScript : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private CheckPointNumber checkPointNumber;
    [SerializeField] private string checkPointInfo;
    [SerializeField] private GameObject nextChekpoint;
    [SerializeField] private float checkPointDuration; //Destroy Duration
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider pointCollider;
    //[SerializeField] private Material pointMaterial;

    //Checker
    GameObject[] enemies;

    private void Awake()
    {
        gameMaster = FindAnyObjectByType<GameMaster>();
    }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        pointCollider = GetComponent<Collider>();
    }

    public void EnemyChecker()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length >= 1)
        {
            meshRenderer.enabled = false;
            pointCollider.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
            pointCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            meshRenderer.enabled = false;
            gameMaster.QuestText = checkPointInfo;
            Destroy(gameObject, checkPointDuration);
        }
    }

    private void OnDestroy()
    {
        if (nextChekpoint != null)
        {
            nextChekpoint.SetActive(true);
        }
        else
        {
            //nextStageTransition
        }
    }

    private void Update()
    {
        EnemyChecker();
    }
}

public enum CheckPointNumber
{
    CheckPoint1, CheckPoint2, CheckPoint3, CheckPoint4, CheckPoint5, CheckPoint6, CheckPoint7
}