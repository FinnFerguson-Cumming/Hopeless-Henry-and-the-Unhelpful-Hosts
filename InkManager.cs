using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkManager : MonoBehaviour
{
    [Header("Ink Settings")]
    public float maxInk = 100f;
    public float currentInk;
    //drain rate per second of drawing
    public float inkDrainRate = 10f;

    [Header("References")]
    public GameManager gameManager;
    public Transform inkBar;
    public GameObject sketchBookPickup;
    public ArmCursor armCursor;

    private bool puzzleFinished = false;
    private Vector3 inkBarInitialScale;

    void Start()
    {
        currentInk = maxInk;
        if (inkBar != null)
        {
            inkBarInitialScale = inkBar.localScale;
        }
    }

    void Update()
    {
       if (!puzzleFinished && currentInk <= 0)
        {
            FinishPuzzle();
        } 
    }

    void FinishPuzzle()
    {
        puzzleFinished = true;
        sketchBookPickup.SetActive(true);

        Cursor.visible = true;
    }

    public void ConsumeInk()
    {
        if (puzzleFinished) return;

        currentInk -= inkDrainRate * Time.deltaTime;
        currentInk = Mathf.Clamp(currentInk, 0, maxInk);

        UpdateInkBar();
    }

    private void UpdateInkBar()
    {
        if (inkBar == null) return;

        float fillPercent = currentInk / maxInk;
        inkBar.localScale = new Vector3(inkBarInitialScale.x, inkBarInitialScale.y * fillPercent, inkBarInitialScale.z);
    }
}