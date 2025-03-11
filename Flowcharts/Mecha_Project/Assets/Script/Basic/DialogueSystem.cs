using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [Header("Dialogue Set Up")]
    [SerializeField] Sprite[] playerProfile;
    [SerializeField] string[] dialogue;
    [SerializeField] AudioClip[] voiceClip;
    [SerializeField] bool isActive;

    [Header("Dialogue ON")]
    [SerializeField] UnityEngine.UI.Image imageProfile;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] AudioSource voiceSource;

    //check
    EnemyModel[] enemyModels;

    //private void Start()
    //{
    //    //StartCoroutine(DialougeActive());
    //}

    IEnumerator EnemyMonitoring()
    {
        enemyModels = FindObjectsOfType<EnemyModel>();
        foreach (EnemyModel enemy in enemyModels)
        {
            if (enemy.isDeath)
            {
                isActive = true;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    IEnumerator DialougeActive()
    {
        int dialougeNumber = Random.Range(0, playerProfile.Length);
        if (isActive)
        {
            voiceSource.enabled = true;
            voiceSource.clip = voiceClip[dialougeNumber];
            imageProfile.sprite = playerProfile[dialougeNumber];
            dialogueText.text = dialogue[dialougeNumber];
            yield return new WaitForSeconds(1f);
            isActive = false;
        }
    }

    void Update()
    {
        StartCoroutine(DialougeActive());
        StartCoroutine(EnemyMonitoring());
    }
}
