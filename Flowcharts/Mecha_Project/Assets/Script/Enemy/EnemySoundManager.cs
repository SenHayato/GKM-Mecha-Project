using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    [Header("Sound Library")]
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip slashSound;
    [SerializeField] AudioClip exploded;

    [Header("Sound Trigger")]
    [SerializeField] bool isFiring = false;
    [SerializeField] bool isSlashing = false;
    [SerializeField] bool isExploded = false;

    [Header("Sound Set Up")]
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] AudioSource soundSource;

    private void Awake()
    {
        soundSource = GetComponentInChildren<AudioSource>();
    }

    void EnemyMonitorRange()
    {
        if (enemyModel.enemyType == EnemyType.EnemyRange)
        {
            soundSource.clip = fireSound;
            if (enemyModel.isAttacking && !isFiring)
            {
                isFiring = true;
                soundSource.Play();
            }

            if (!enemyModel.isAttacking)
            {
                isFiring = false;
            }
        }
    }

    void EnemyShortMonitor()
    {
        if (enemyModel.enemyType == EnemyType.EnemyShort)
        {
            soundSource.clip = slashSound;
            if (enemyModel.isAttacking && !isSlashing)
            {
                isSlashing = true;
                soundSource.Play();
            }

            if (!enemyModel.isAttacking)
            {
                isSlashing = false;
            }
        }
    }

    private void Update()
    {
        EnemyMonitorRange();
        EnemyShortMonitor();
    }
}
