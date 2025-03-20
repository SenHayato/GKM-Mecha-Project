using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour
{
    [Header("CutScene Video")]
    [SerializeField] VideoClip introCutScene;
    [SerializeField] VideoClip stage1CutScene;
    [SerializeField] VideoClip stage2CutScene;
    [SerializeField] VideoClip stage3CutScene;
    [SerializeField] VideoClip outroCutScene;

    [Header("Video Set Up")]
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] bool isPlaying = false;
}
