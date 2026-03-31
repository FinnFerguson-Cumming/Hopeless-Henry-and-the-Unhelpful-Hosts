using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CheckPassword : MonoBehaviour
{
    public GameObject[] slots; //make sure to assign all of these slots in the inspector!

    [Header("Ink JSON")]
    [SerializeField] private TextAsset passwordSuccessWithoutTimeJSON;
    [SerializeField] private TextAsset passwordFailureJSON;

    private string correctPassword = "LETMEPAST";
    private bool hasCheckedPassword = false;
    public bool puzzleLocked = false;

    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }
    }

    void Update()
    {
        //Debug.Log("CheckPassword Update() is running");
        if (!hasCheckedPassword && AllSlotsFilled())
        {
            string attempt = ReadPassword();

            hasCheckedPassword = true;

            if (attempt == correctPassword)
            {
                gameManager.enteredPassword = true;
                DialogueManager.GetInstance().EnterDialogueMode(passwordSuccessWithoutTimeJSON);
                puzzleLocked = true;
                DisableDragging();
            }
            else
            {
                gameManager.enteredPassword = false;
                DialogueManager.GetInstance().EnterDialogueMode(passwordFailureJSON);
            }
        }

        if (hasCheckedPassword && !AllSlotsFilled())
        {
            hasCheckedPassword = false;
        }
    }    

    bool AllSlotsFilled()
    {
        bool allFilled = true;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount == 0)
            {
                allFilled = false;
            }
        }

        return allFilled;
    }

    string ReadPassword()
    {
        string result = "";

        foreach (GameObject slot in slots)
        {
            GameObject letterTile = slot.transform.GetChild(0).gameObject;
            LetterTile letterData = letterTile.GetComponent<LetterTile>();
            if (letterData != null)
            {
                result += letterData.letter.ToUpper();
            }
            else
            {
                Debug.LogWarning("Missing LetterTile component on letter.");
                result += "?";
            }
        }

        return result;
    }

    void DisableDragging()
    {
        //finds all of the draggableletter scripts in the scene and disables them
        DraggableLetter[] allLetters = FindObjectsOfType<DraggableLetter>();
        foreach (var letter in allLetters)
        {
            letter.enabled = false;
        }
    }

    public void CheckNow()
    {
        //don't recheck once the password is solved
        if (puzzleLocked) return;

        if (AllSlotsFilled())
        {
            string attempt = ReadPassword();

            if (attempt == correctPassword)
            {
                gameManager.enteredPassword = true;
                DialogueManager.GetInstance().EnterDialogueMode(passwordSuccessWithoutTimeJSON);
                puzzleLocked = true;
                DisableDragging();
            }
            else
            {
                gameManager.enteredPassword = false;
                DialogueManager.GetInstance().EnterDialogueMode(passwordFailureJSON);
            }
        }
    }
}