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

    private void Awake()
    {
        hudManager = FindFirstObjectByType<HUDGameManager>();
    }

    public void QuestMonitor()
    {
        if (stageType == StageType.StageTutorial)
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
