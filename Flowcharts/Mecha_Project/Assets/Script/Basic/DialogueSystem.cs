using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [Header("Dialogue Set Up")]
    [SerializeField] UnityEngine.UI.Image[] playerProfile;
    [SerializeField] TextMeshProUGUI[] dialogueText;

    [SerializeField] EnemyModel[] enemyModels;

    void DialogueSetUp()
    {

    }

    void EnemyMonitoring()
    {
        enemyModels = FindObjectsOfType<EnemyModel>();
    }

    void Update()
    {
        EnemyMonitoring();
    }
}
