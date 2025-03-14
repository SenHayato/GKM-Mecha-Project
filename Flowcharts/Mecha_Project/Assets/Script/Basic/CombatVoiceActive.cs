using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatVoiceActive : MonoBehaviour
{
    [Header("Attack Voice")]
    [SerializeField] AudioClip[] attackVoice;

    [Header("Defence Voice")]
    [SerializeField] AudioClip[] defenceVoice;

    [Header("SkillVoice")]
    [SerializeField] AudioClip skill1Voice;
    [SerializeField] AudioClip skill2Voice;

    [Header("Ultimate Voice")]
    [SerializeField] AudioClip[] ultimateVoice;

    [Header("Damage Voice")]
    [SerializeField] AudioClip[] damageVoice;

    [Header("PowerUp Get Voice")]
    [SerializeField] AudioClip[] powerUpVoice;

    [Header("Voice Set Up")]
    [SerializeField] AudioSource voiceSource;
    [SerializeField] bool isActive;

    //check
    bool wasActive = false;

    private void Awake()
    {
        voiceSource = GetComponent<AudioSource>();
    }
}
