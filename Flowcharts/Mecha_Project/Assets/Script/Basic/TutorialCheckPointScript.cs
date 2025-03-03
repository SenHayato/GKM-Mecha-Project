using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurotialCheckPointScript : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private CheckPointNumber checkPointNumber;
    [SerializeField] private string checkPointInfo;

    private void Awake()
    {
        gameMaster = FindAnyObjectByType<GameMaster>();
    }

    private void QuestDetail()
    {
        switch (checkPointNumber)
        {
            case CheckPointNumber.CheckPoint1:
                checkPointInfo = "Movement Tutorial";
                break;
            case CheckPointNumber.CheckPoint2:
                checkPointInfo = "Evade dan Dash";
                break;
            case CheckPointNumber.CheckPoint3:
                checkPointInfo = "Menembak";
                break;
            case CheckPointNumber.CheckPoint4:
                checkPointInfo = "PowerUp";
                break;
            case CheckPointNumber.CheckPoint5:
                checkPointInfo = "Skill";
                break;
            case CheckPointNumber.CheckPoint6:
                checkPointInfo = "Ultimate";
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameMaster.QuestText = checkPointInfo;
        }
    }

    private void Update()
    {
        QuestDetail();
    }
}

public enum CheckPointNumber
{
    CheckPoint1, CheckPoint2, CheckPoint3, CheckPoint4, CheckPoint5, CheckPoint6,
}