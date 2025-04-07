using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CGManager : MonoBehaviour
{
    [Header("CG Video SetUp")]
    [SerializeField] VideoClip[] CGClips;
    [SerializeField] bool isPlaying;

    [Header("VideoPlayer SetUp")]
    [SerializeField] GameObject videoPlayerCanvas;
    [SerializeField] VideoPlayer videoPlayer;

    //flag
    bool wasPlaying = false;

    private void Start()
    {
        videoPlayerCanvas.SetActive(false);
    }

    public void CGButton(int playCGNumber)
    {
        videoPlayer.clip = CGClips[playCGNumber - 1];
        Debug.Log("Video ke "+playCGNumber);
        videoPlayerCanvas.SetActive(true);
        isPlaying = true;
    }

    void VideoSetUp()
    {
        if (!wasPlaying && isPlaying)
        {
            wasPlaying = true;
            StartCoroutine(VideoPlaying());
        }
    }

    IEnumerator VideoPlaying()
    {
        if (isPlaying)
        {
            videoPlayer.Play();
            yield return new WaitForSeconds((float)videoPlayer.clip.length);
            videoPlayerCanvas.SetActive(false);
            videoPlayer.clip = null;
            isPlaying = false;
            wasPlaying = false;
        }
    }

    private void Update()
    {
        VideoSetUp();
    }
}
