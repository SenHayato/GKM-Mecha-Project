using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CombatVoiceActive : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] MechaPlayer mechaPlayer;
    //[SerializeField] PlayerActive playerActive;

    [Header("Attack Voice")]
    [SerializeField] AudioClip[] attackVoice;

    [Header("Defence Voice")]
    [SerializeField] AudioClip[] defenceVoice;

    [Header("SkillVoice")]
    [SerializeField] AudioClip skill1Voice;
    [SerializeField] AudioClip skill2Voice;

    [Header("Damage Voice")]
    [SerializeField] AudioClip[] damageVoice;

    [Header("PowerUp Get Voice")]
    [SerializeField] AudioClip[] powerUpVoice;

    [Header("Voice Set Up")]
    [SerializeField] AudioSource voiceSource;
    [SerializeField] bool isActive = false;

    //check
    bool shootActive = false;
    bool defenceActive = false;

    private void Awake()
    {
        //playerActive = GetComponent<PlayerActive>();
        mechaPlayer = GetComponent<MechaPlayer>();
    }

    private void Start()
    {
        voiceSource = GetComponent<AudioSource>();
    }

    void PlayerMonitoring()
    {
        int voiceNumber = Random.Range(0, attackVoice.Length);
        if (mechaPlayer.isShooting && !shootActive)
        {
            shootActive = true;
            voiceSource.clip = attackVoice[voiceNumber];
            voiceSource.Play();
        }

        if (mechaPlayer.isBlocking && !defenceActive)
        {
            defenceActive = true;
            voiceSource.clip = defenceVoice[voiceNumber];
            voiceSource.Play();
        }

        //Reset
        if (!mechaPlayer.isShooting)
        {
            shootActive = false;
        }

        if (!mechaPlayer.isBlocking)
        {
            defenceActive = false;
        }
    }

    public void PowerUpGet()
    {
        int voiceNumber = Random.Range(0, powerUpVoice.Length);
        voiceSource.clip = powerUpVoice[voiceNumber];
        voiceSource.Play();
    }

    public void DamageVoice()
    {
        int voiceNumber = Random.Range(0, damageVoice.Length);
        voiceSource.clip = damageVoice[voiceNumber];
        voiceSource.Play();
    }

    IEnumerator SKill1Voice()
    {
        if (mechaPlayer.usingSkill1 && !isActive)
        {
            isActive = true;
            voiceSource.clip = skill1Voice;
            voiceSource.Play();
            yield return new WaitForSeconds(mechaPlayer.skill1Duration + 2f);
            isActive = false;
        }
    }
    
    IEnumerator Skill2Voice()
    {
        if (mechaPlayer.usingSkill2 && !isActive)
        {
            isActive = true;
            voiceSource.clip = skill2Voice;
            voiceSource.Play();
            yield return new WaitForSeconds(mechaPlayer.skill2Duration + 2f);
            isActive = false;
        }
    }

    private void Update()
    {
        PlayerMonitoring();
        StartCoroutine(SKill1Voice());
        StartCoroutine(Skill2Voice());
    }
}
