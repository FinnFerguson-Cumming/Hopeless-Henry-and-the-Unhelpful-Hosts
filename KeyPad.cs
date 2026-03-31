using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyPad : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset phoneSuccessJSON;
    [SerializeField] private TextAsset phoneFailureJSON;
    [SerializeField] private TextAsset devilJSON;
    [SerializeField] private TextAsset ambulanceCallDisconnected;
    [SerializeField] private TextAsset callCannotBeConnected;

    public TMP_InputField charHolder;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;
    public GameObject button5;
    public GameObject button6;
    public GameObject button7;
    public GameObject button8;
    public GameObject button9;
    public GameObject button0;
    public GameObject clearButton;
    public GameObject enterButton;
    GameManager gameManager;
    ClickManager clickManager;
    ItemData itemData;
    SpriteAnimator spriteAnimator;
    NameTag nameTag;
    AnimationData animationData;
    DialogueManager dialogueManager;
    DialogueTrigger dialogueTrigger;
    public bool ambulanceDialed = false;

    private bool canPressEnter = true;
    [SerializeField] private float enterCooldown = 1.0f;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        clickManager = FindObjectOfType<ClickManager>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        dialogueTrigger = FindObjectOfType<DialogueTrigger>();
    }

    public void b1()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "1";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }
    }

    public void b2()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "2";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        } 
    }

    public void b3()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "3";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }
    }

    public void b4()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "4";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }   
    }

    public void b5()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "5";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }
    }

    public void b6()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "6";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }
    }

    public void b7()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "7";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }  
    }

    public void b8()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "8";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }
    }

    public void b9()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "9";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }
    }

    public void b0()
    {
        if (charHolder.text.Length < 10)
        {
            charHolder.text = charHolder.text + "0";
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
        }   
    }

    public void clearEvent()
    {
        charHolder.text = null;
        //gameManager.PlaySound(GameManager.soundsNames.clearButton);
    }

    public void enterEvent()
    {
        if (!canPressEnter) return;
        StartCoroutine(EnterCooldownRoutine());

        if (charHolder.text == "7213")
        {
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
            DialogueManager.GetInstance().EnterDialogueMode(phoneSuccessJSON);
            gameManager.receivedPhoneHint = true;
            gameManager.phoneDialled = true;
            clearEvent();
        }
        else if (charHolder.text == "666")
        {
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
            DialogueManager.GetInstance().EnterDialogueMode(devilJSON);
            clearEvent();
        }
        else if (charHolder.text == "000" && ambulanceDialed == false)
        {
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
            gameManager.PlaySound(GameManager.soundsNames.ambulanceSiren);
            ambulanceDialed = true;
            clearEvent();
        }
        else if (charHolder.text == "000" && ambulanceDialed == true)
        {
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
            DialogueManager.GetInstance().EnterDialogueMode(ambulanceCallDisconnected);
            clearEvent();
            /*DialogueManager.GetInstance().onDialogueExit = () =>
            {
                Debug.Log("exit detected");
                StartCoroutine(gameManager.ChangeScene(2, 0));
            };*/
        }
        else if (charHolder.text == "911" || charHolder.text == "999" || charHolder.text == "112" || charHolder.text == "110")
        {
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
            DialogueManager.GetInstance().EnterDialogueMode(callCannotBeConnected);
            clearEvent();
        }
        else
        {
            gameManager.PlaySound(GameManager.soundsNames.phoneButton);
            DialogueManager.GetInstance().EnterDialogueMode(phoneFailureJSON);
            clearEvent();
        }
    }

    private IEnumerator EnterCooldownRoutine()
    {
        canPressEnter = false;
        yield return new WaitForSeconds(enterCooldown);
        canPressEnter = true;
    }

    public void OnEnable()
    {
        charHolder.text = null;
    }
}