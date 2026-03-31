using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordsMenu : MonoBehaviour
{
    public static RecordsMenu Instance;

    [Header("UI")]
    [SerializeField] private GameObject menuPanel;

    [Header("Dialogue")]
    [SerializeField] private TextAsset recordsSuccessJSON;

    private GameManager gameManager;

    public enum RecordType
    {
        PoorlyLitTriangle,
        HighwayToBell,
        RillieStylish,
        OftenTimes
    }

    private void Awake()
    {
        Instance = this;
        gameManager = FindObjectOfType<GameManager>();
        menuPanel.SetActive(false);
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
    }

    public void SelectRecord(RecordType record)
    {
        CloseMenu();

        //fade from Art Room music to record
        AudioSource artRoom = gameManager.artRoomSoundtrack;
        AudioSource recordTrack = GetRecordTrack(record);

        gameManager.FadeToTrack(artRoom, recordTrack);

        //play the record immediately
        recordTrack.Play();

        //sound effect
        gameManager.PlaySound(GameManager.soundsNames.recordNeedle);

        //start dialogue at the same time
        DialogueManager.GetInstance().EnterDialogueMode(recordsSuccessJSON);

        //when record ends, fade back
        StartCoroutine(ReturnToArtRoom(recordTrack));
    }

    private AudioSource GetRecordTrack(RecordType record)
    {
        switch (record)
        {
            case RecordType.PoorlyLitTriangle: return gameManager.poorlyLitTriangleSoundtrack;
            case RecordType.HighwayToBell: return gameManager.highwayToBellSoundtrack;
            case RecordType.RillieStylish: return gameManager.rillieStylishSoundtrack;
            case RecordType.OftenTimes: return gameManager.oftenTimesSoundtrack;
            default: return null;
        }
    }

    private IEnumerator ReturnToArtRoom(AudioSource recordTrack)
    {
        yield return new WaitForSeconds(recordTrack.clip.length);
        gameManager.FadeToTrack(recordTrack, gameManager.artRoomSoundtrack);
    }
}