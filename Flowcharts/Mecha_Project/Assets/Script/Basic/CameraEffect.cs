using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Volume boostVolume;

    [Header("Reference")]
    [SerializeField] private MechaPlayer mechaPlayer;

    private void Awake()
    {
        mechaPlayer = FindFirstObjectByType<MechaPlayer>();
        criticalVolume = criticalEffect.GetComponent<Volume>();
        boostVolume = boostEffect.GetComponent<Volume>();
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
            StartCoroutine(SmoothBoost());
        }
        else
        {
            StopCoroutine(SmoothBoost());
            boostEffect.SetActive(false);
        }
    }

    public IEnumerator SmoothBoost()
    {
        if (boostVolume.profile.TryGet<UnityEngine.Rendering.Universal.LensDistortion>(out var lensDistortion))
        {
            lensDistortion.intensity.overrideState = true;
            while (mechaPlayer.isBoosting)
            {
                lensDistortion.intensity.value = Mathf.Lerp(0, -0.25f, 1f);
                yield return null;
            }
            lensDistortion.intensity.value = 0f;
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
