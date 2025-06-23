using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDialogueSystem : MonoBehaviour
{
    [Header("Dialogue Set Up")]
    [SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] Animation animationClip;

    [Header("Kill Dialogue")]
    [SerializeField] Sprite[] killSprite;
    [SerializeField] string[] killDialogue;
    [SerializeField] AudioClip[] killVoiceClip;

    [Header("Ultimate Dialogue")]
    [SerializeField] Sprite[] ultimateSprite;
    [SerializeField] string[] ultimateDialogue;
    [SerializeField] AudioClip[] ultimateVoiceClip;

    [Header("Critical Dialogue")]
    [SerializeField] Sprite[] crticalSprite;
    [SerializeField] string[] criticalDialogue;
    [SerializeField] AudioClip[] criticalVoiceClip;

    //Setiap Stage Berbeda
    [Header("Special Event")]                       //sprite, text dan voice harus terurut
    [SerializeField] Sprite specialSprite;
    [SerializeField] string specialDialouge;
    [SerializeField] AudioClip specialVoiceClip;

    [Header("Dialogue ON")]
    [SerializeField] bool killTrigger;
    [SerializeField] bool ultimateTrigger;
    [SerializeField] bool criticalTrigger;
    [SerializeField] UnityEngine.UI.Image imageProfile;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] AudioSource voiceSource;

    //check
    EnemyModel[] enemyModels;
    bool wasActive = false;
    bool killWasActive = false;

    private void Awake()
    {
        animationClip = GetComponent<Animation>();
    }

    private void Start()
    {
        mechaPlayer = FindFirstObjectByType<MechaPlayer>();
        StartCoroutine(HealthMonitor());
        StartCoroutine(SpecialDialougue());
    }

    IEnumerator PlayerMonitoring()
    {
        if (mechaPlayer.UsingUltimate)
        {
            ultimateTrigger = true;
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator HealthMonitor()
    {
        while (true)
        {
            if (mechaPlayer.Health <= 25000)
            {
                if (!criticalTrigger) // Hindari mengulang
                {
                    criticalTrigger = true;
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                criticalTrigger = false;
                killWasActive = false;
            }
            yield return new WaitForSeconds(0.5f); // Cek
        }
    }

    IEnumerator EnemyDeathMonitoring()
    {
        enemyModels = FindObjectsOfType<EnemyModel>();
        foreach (EnemyModel enemy in enemyModels)
        {
            if (enemy.isDeath)
            {
                killTrigger = true;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    IEnumerator CriticalDialouge()
    {
        int dialougeNumber = Random.Range(0, crticalSprite.Length);
        if (criticalTrigger && !killWasActive)
        {
            killWasActive = true;
            animationClip.Play("DialogueIn");
            voiceSource.enabled = true;
            voiceSource.clip = criticalVoiceClip[dialougeNumber];
            voiceSource.Play();
            imageProfile.sprite = crticalSprite[dialougeNumber];
            dialogueText.text = criticalDialogue[dialougeNumber];
            yield return new WaitForSeconds(criticalVoiceClip[dialougeNumber].length);
            animationClip.Play("DialogueOut");
            yield return new WaitForSeconds(0.8f);
            criticalTrigger = false; //flag
            //killWasActive = false;
        }
    }

    IEnumerator UltimateDialouge()
    {
        int dialougeNumber = Random.Range(0, ultimateSprite.Length);
        if (ultimateTrigger && !wasActive)
        {
            wasActive = true;
            animationClip.Play("DialogueIn");
            voiceSource.enabled = true;
            voiceSource.clip = ultimateVoiceClip[dialougeNumber];
            voiceSource.Play();
            imageProfile.sprite = ultimateSprite[dialougeNumber];
            dialogueText.text = ultimateDialogue[dialougeNumber];
            yield return new WaitForSeconds(mechaPlayer.UltDuration);
            animationClip.Play("DialogueOut");
            yield return new WaitForSeconds(0.8f);
            ultimateTrigger = false;
            wasActive = false;
        }
    }

    IEnumerator KillDialougeActive()
    {
        int dialougeNumber = Random.Range(0, killSprite.Length); // 0 < 3 dalam array atau 1 < 4 normal
        if (killTrigger && !wasActive)
        {
            wasActive = true;
            animationClip.Play("DialogueIn");
            voiceSource.enabled = true;
            voiceSource.clip = killVoiceClip[dialougeNumber];
            voiceSource.Play();
            imageProfile.sprite = killSprite[dialougeNumber];
            dialogueText.text = killDialogue[dialougeNumber];
            yield return new WaitForSeconds(killVoiceClip[dialougeNumber].length);
            animationClip.Play("DialogueOut");
            yield return new WaitForSeconds(0.8f);
            killTrigger = false;
            wasActive = false;
        }
    }

    IEnumerator SpecialDialougue()
    {
        if (!wasActive)
        {
            wasActive = true;
            animationClip.Play("DialogueIn");
            voiceSource.enabled = true;
            voiceSource.clip = specialVoiceClip;
            voiceSource.Play();
            imageProfile.sprite = specialSprite;
            dialogueText.text = specialDialouge;
            yield return new WaitForSeconds(specialVoiceClip.length);
            animationClip.Play("DialogueOut");
            yield return new WaitForSeconds(0.8f);
            wasActive = false;
        }
    }

    void Update()
    {
        //StartCoroutine(HealthMonitor());
        StartCoroutine(CriticalDialouge());
        StartCoroutine(KillDialougeActive());
        StartCoroutine(EnemyDeathMonitoring());
        StartCoroutine(PlayerMonitoring());
        StartCoroutine(UltimateDialouge());
    }
}
