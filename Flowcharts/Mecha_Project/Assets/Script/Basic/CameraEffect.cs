using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffect : MonoBehaviour
{
    [Header("Effect Library")]
    [SerializeField] private GameObject boostEffect;
    [SerializeField] private GameObject ultimateEffect;
    [SerializeField] private GameObject criticalEffect;
    [SerializeField] private GameObject scopeEffect;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject awakeningEffect;

    [Header("EffectVolume")]
    [SerializeField] private Volume criticalVolume;
    [SerializeField] private Volume boostVolume;
    //[SerializeField] private Volume hitVolume;

    [Header("Reference")]
    [SerializeField] private MechaPlayer mechaPlayer;

    private void Awake()
    {
        mechaPlayer = FindFirstObjectByType<MechaPlayer>();
        criticalVolume = criticalEffect.GetComponent<Volume>();
        boostVolume = boostEffect.GetComponent<Volume>();
        //hitVolume = hitEffect.GetComponent<Volume>();
    }

    private void Start()
    {
        boostEffect.SetActive(false);
        ultimateEffect.SetActive(false);
        criticalEffect.SetActive(false);
        scopeEffect.SetActive(false);
        hitEffect.SetActive(false);
        awakeningEffect.SetActive(false);
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
                vignette.intensity.value = Mathf.PingPong(Time.time * 1f, 0.3f); // Berkedip antara 0 dan 0.5
                yield return null;
            }
            vignette.intensity.value = 0f; // Kembali ke normal setelah kedipan
        }
    }

    public IEnumerator HitEffect() //Masih bug
    {
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        hitEffect.SetActive(false);
    }

    public void BoostEffect()
    {
        if (mechaPlayer.isBoosting || mechaPlayer.usingSkill1)
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

    void AwakeningEffect()
    {
        if (mechaPlayer.UsingAwakening)
        {
            awakeningEffect.SetActive(true);
        }
        else
        {
            awakeningEffect.SetActive(false);
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

    public void ScopeCameraEffect()
    {
        if (mechaPlayer.isAiming)
        {
            scopeEffect.SetActive(true);
        }
        else
        {
            scopeEffect.SetActive(false);
        }
    }

    private void Update()
    {
        UltimateEffect();
        BoostEffect();
        CriticalEffect();
        ScopeCameraEffect();
        AwakeningEffect();
    }
}
