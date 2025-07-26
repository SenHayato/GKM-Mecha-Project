using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class CGManager : MonoBehaviour
{
    [Header("CG Video SetUp")]
    [SerializeField] VideoClip[] CGClips;
    [SerializeField] bool isPlaying;
    [SerializeField] KeyCode skipButton;

    [Header("VideoPlayer SetUp")]
    [SerializeField] GameObject videoPlayerCanvas;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] AudioSource audioSource;

    [SerializeField] float CGDuration;
    //flag
    bool wasPlaying = false;

    private void Start()
    {
        videoPlayerCanvas.SetActive(false);
    }

    public void CGButton(int playCGNumber)
    {
        videoPlayer.clip = CGClips[playCGNumber - 1];
        videoPlayerCanvas.SetActive(true);
        CGDuration = (float)CGClips[playCGNumber - 1].length;
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
        if (Input.GetKeyDown(skipButton) || Gamepad.current != null && Gamepad.current.buttonEast.isPressed) yield break;

        if (isPlaying)
        {
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            videoPlayer.Play();
            yield return new WaitForSeconds(CGDuration);
            videoPlayer.clip = null;
            isPlaying = false;
            wasPlaying = false;
            //Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            videoPlayerCanvas.SetActive(false);
        }
    }

    void SkipCG()
    {
        if (isPlaying && Input.GetKeyDown(skipButton) || Gamepad.current != null && Gamepad.current.buttonEast.isPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            videoPlayer.Stop();
            isPlaying = false;
            wasPlaying = false;
            videoPlayer.clip = null;
            videoPlayerCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        SkipCG();
        VideoSetUp();
        if (videoPlayer.clip != null)
        {
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }
}
