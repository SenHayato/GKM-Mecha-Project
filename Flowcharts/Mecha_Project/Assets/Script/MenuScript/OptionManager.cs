using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] AudioMixer sfxMixer;

    [Header("Audio Slider")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    private void Start()
    {
        bgmSlider.onValueChanged.AddListener(VolumeMonitor);
    }

    void VolumeMonitor(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        bgmMixer.SetFloat("MasterVolume", dB);
    }

    //private void Update()
    //{
    //    //VolumeMonitor();
    //}

}
