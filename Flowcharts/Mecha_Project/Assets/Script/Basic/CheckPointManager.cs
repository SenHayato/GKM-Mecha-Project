using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [Header("Stage Name")]
    public StageType stageType;

    [Header("CheckPoint Atribut")]
    [SerializeField] private int checkPointReach;
    [SerializeField] private string questInfo;
    [SerializeField] private HUDGameManager hudManager;
    [SerializeField] private GameMaster gameManager;

    private void Awake()
    {
        hudManager = FindFirstObjectByType<HUDGameManager>();
        gameManager = FindFirstObjectByType<GameMaster>();
    }

    public void QuestMonitor()
    {
        stageType = gameManager.StageType;
        if (stageType == StageType.StageTutorial)
        {
            
        }

        if (stageType == StageType.Stage1)
        {

        }
    }

    public void QuestText()
    {
        hudManager.questInfo.text = questInfo;
    }
    
    void Update()
    {
        QuestMonitor();
        QuestText();
    }
}
