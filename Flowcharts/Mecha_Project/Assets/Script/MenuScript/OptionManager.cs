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

    void VolumeMonitor()
    {
        bgmMixer.SetFloat("MusicVolume", Mathf.Log10(bgmSlider.value) * 20f);
        sfxMixer.SetFloat("SoundVolume", sfxSlider.value);
    }

    private void Update()
    {
        VolumeMonitor();
    }
}
