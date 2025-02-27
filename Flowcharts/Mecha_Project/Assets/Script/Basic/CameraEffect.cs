using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffect : MonoBehaviour
{
    [Header("Effect Library")]
    [SerializeField] private GameObject boostEffect;
    [SerializeField] private GameObject ultimateEffect;
    [SerializeField] private GameObject criticalEffect;

    [Header("EffectVolume")]
    [SerializeField] private Volume criticalVolume;

    [Header("Reference")]
    [SerializeField] private MechaPlayer mechaPlayer;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        mechaPlayer = FindFirstObjectByType<MechaPlayer>();
        criticalVolume = criticalEffect.GetComponent<Volume>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        boostEffect.SetActive(false);
        ultimateEffect.SetActive(false);
        criticalEffect.SetActive(false);
    }

    public void CriticalEffect()
    {
        if (mechaPlayer.Health <= 25000)
        {
            criticalEffect.SetActive(true);
            StartCoroutine(CriticalBlink());
        }
        else
        {
            StopCoroutine(CriticalBlink());
            criticalEffect.SetActive(false);
        }
    }

    private IEnumerator CriticalBlink()
    {
        if (criticalVolume.profile.TryGet<UnityEngine.Rendering.Universal.Vignette>(out var vignette))
        {
            vignette.intensity.overrideState = true; // Pastikan bisa diubah
            while (mechaPlayer.Health <= 25000f)
            {
                //elapsedTime += Time.deltaTime;
                vignette.intensity.value = Mathf.PingPong(Time.time * 1f, 0.5f); // Berkedip antara 0 dan 0.5
                yield return null;
            }
            vignette.intensity.value = 0f; // Kembali ke normal setelah kedipan
        }
    }

    public void BoostEffect()
    {
        if (mechaPlayer.isBoosting)
        {
            boostEffect.SetActive(true);
            anim.Play("BlurEffect");
        }
        else
        {
            boostEffect.SetActive(false);
        }
    }

    public void UltimateEffect()
    {
        if (mechaPlayer.UsingUltimate)
        {
            ultimateEffect.SetActive(true);
        }
        else
        {
            ultimateEffect.SetActive(false);
        }
    }

    private void Update()
    {
        UltimateEffect();
        BoostEffect();
        CriticalEffect();
    }
}
