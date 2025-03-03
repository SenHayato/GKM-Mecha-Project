using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurotialCheckPointScript : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private CheckPointNumber checkPointNumber;
    [SerializeField] private string checkPointInfo;
    [SerializeField] private GameObject nextChekpoint;
    [SerializeField] private float checkPointDuration;
    [SerializeField] private MeshRenderer meshRenderer;

    private void Awake()
    {
        gameMaster = FindAnyObjectByType<GameMaster>();
    }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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
}

public enum CheckPointNumber
{
    CheckPoint1, CheckPoint2, CheckPoint3, CheckPoint4, CheckPoint5, CheckPoint6, CheckPoint7, CheckPoint8, CheckPoint9, CheckPoint10,
}