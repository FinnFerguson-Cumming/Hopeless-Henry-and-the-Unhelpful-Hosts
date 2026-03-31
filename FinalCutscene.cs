using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class FinalCutscene : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public GameObject videoPlayerUIImage;
    GameManager gameManager;

    public void PlayFinalCutscene()
    {
        if (videoPlayerUIImage != null)
        {
            //show the video UI
            videoPlayerUIImage.SetActive(true);
        }

        if (VideoPlayer != null)
        {
            //starts the video
            VideoPlayer.Play();
        }
    }

    void Start()
    {
        VideoPlayer.loopPointReached += EndReached;
        gameManager = FindObjectOfType<GameManager>();
    }

    void EndReached(VideoPlayer vp)
    {
        Debug.Log("Final Cutscene watched, working");
        gameManager.watchedEndCutscene = true;
    }
}