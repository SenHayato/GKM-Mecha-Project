using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDialogueSystem : MonoBehaviour
{
    [Header("Dialogue Set Up")]
    [SerializeField] Sprite[] playerProfile;
    [SerializeField] string[] dialogue;
    [SerializeField] AudioClip[] voiceClip;
    [SerializeField] Animation animationClip;

    [Header("Dialogue ON")]
    [SerializeField] bool isActive;
    [SerializeField] UnityEngine.UI.Image imageProfile;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] AudioSource voiceSource;

    //check
    EnemyModel[] enemyModels;
    bool wasActive;

    //private void Start()
    //{
    //    StartCoroutine(DialougeActive());
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
        if (isActive && !wasActive)
        {
            wasActive = true;
            animationClip.Play("DialogueIn");
            voiceSource.enabled = true;
            voiceSource.clip = voiceClip[dialougeNumber];
            voiceSource.Play();
            imageProfile.sprite = playerProfile[dialougeNumber];
            dialogueText.text = dialogue[dialougeNumber];
            yield return new WaitForSeconds(voiceClip[dialougeNumber].length);
            animationClip.Play("DialogueOut");
            yield return new WaitForSeconds(0.8f);
            isActive = false;
            wasActive = false;
        }
    }

    void Update()
    {
        StartCoroutine(DialougeActive());
        StartCoroutine(EnemyMonitoring());
    }
}
