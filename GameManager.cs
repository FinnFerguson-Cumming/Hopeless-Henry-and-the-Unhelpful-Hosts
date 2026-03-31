using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    ClickManager clickManager;
    public static List<ItemData> collectedItems = new List<ItemData>();
    static float moveSpeed = 5f, moveAccuracy = 0.05f;
    public enum soundsNames
    {
        none,
        click,
        use,
        vaseSmash,
        startButton,
        clockTicking,
        doorOpen,
        titleAmbience,
        unlockingDoor,
        vipSuccess,
        portraitReveal,
        itemPickup,
        phoneButton,
        clearButton,
        notebookOpen,
        notebookFinish,
        clockAt12,
        lavaLampClonk,
        drawerOpening,
        secondPianoSetup,
        lightsOnVIP,
        splashPickup,
        ambulanceSiren,
        inventoryOpen,
        recordNeedle
    }

    public soundsNames testSounds;

    [Header("Setup")]
    public AnimationData[] playerAnimations;
    public GameObject pausePanel;
    public static bool isPaused = false;
    public RectTransform nameTag, hintBox;
    public Image blockingImage;
    public GameObject blockingImage2;
    public Texture2D mainCursor;
    public Texture2D greenCursor;
    public Texture2D redCursor;
    [SerializeField] private ArmCursor armCursor;
    public GameObject[] localScenes;
    public int activeLocalScene = 0;
    public Transform[] playerStartPositions;
    public bool isFinalScene = false;
    PartyEntryManager partyEntryManager;
    public GameObject gameCompletionG;
    
    [Header("Henry")]
    public Vector3 henryLastPosition;
    public Transform henryTransform;
    public GameObject henry;

    [Header("Audio")]
    public AudioSource titleScreenSoundtrack;
    public AudioSource hallwaySoundtrack;
    public AudioSource poolRoomSoundtrack;
    public AudioSource artRoomSoundtrack;
    public AudioSource musicRoomSoundtrack;
    public AudioSource vipRoomSoundtrack;
    public AudioSource poorlyLitTriangleSoundtrack;
    public AudioSource highwayToBellSoundtrack;
    public AudioSource rillieStylishSoundtrack;
    public AudioSource oftenTimesSoundtrack;

    private float titleScreenVolume = 0.15f;
    private float hallwayVolume = 0.15f;
    private float poolRoomVolume = 0.15f;
    private float artRoomVolume = 0.15f;
    private float musicRoomVolume = 0.15f;

    //records volume
    private float poorlyLitVolume = 0.05f;
    private float highwayToBellVolume = 0.05f;
    private float rillieStylishVolume = 0.05f;
    private float oftenTimesVolume = 0.05f;

    public float fadeTime;
    
    public AudioClip[] soundEffects;
    
    private Dictionary<int, Vector2> henryLastPositionsByScene = new Dictionary<int, Vector2>();

    [Header("Puzzle Completion Status")]
    public GameObject poolRoomPaintingReveal;
    public bool hasStartedPoolRoomCheck;
    public GameObject artRoomPaintingReveal;
    public bool hasStartedArtRoomCheck;
    public GameObject musicRoomPaintingReveal;
    public bool hasStartedMusicRoomCheck;
    public GameObject poolRoomPaintingRevealCutaway;
    public GameObject artRoomPaintingRevealCutaway;
    public GameObject musicRoomPaintingRevealCutaway;
    public GameObject powerCordGiven;
    public GameObject secondaryPianoCutaway;
    public GameObject lavaLampCutaway;
    public GameObject sheetMusicCutaway;

    [Header("Equipment")]
    public GameObject equipmentCanvas;
    public Image[] equipmentSlots, equipmentImages;
    public Sprite[] completeEquipmentImages;
    public Sprite emptyItemSlotSprite;
    public Color defaultSlotColor;
    public Color selectedItemColor;
    public int selectedCanvasSlotID = 0, selectedItemID;
    public GameObject hatSprite;
    public AnimationData hatOpenAnimation;
    public AnimationData hatPickupClosedAnimation;
    public AnimationData hatPickupOpenAnimation;
    public AnimationData hatIdleOpenAnimation;
    public AnimationData hatIdleClosedAnimation;
    public AnimationData hatOpenForwards;
    public AnimationData hatOpenBackwards;

    [Header("Ink JSON (Pool Room)")]
    [SerializeField] private TextAsset phoneSuccess;
    [SerializeField] private TextAsset phoneFailure;
    [SerializeField] private TextAsset bookcaseSuccess;
    [SerializeField] private TextAsset bookcaseFailure;
    [SerializeField] private TextAsset henryEntersPR;
    [SerializeField] private TextAsset painting;
    [SerializeField] private TextAsset poolEquipment;
    [SerializeField] private TextAsset vaseSmash;
    [SerializeField] private TextAsset chandelier;
    [SerializeField] private TextAsset window;
    [SerializeField] private TextAsset poolTable;

    [Header("Ink JSON (Music Room)")]
    [SerializeField] private TextAsset henryEntersMR;
    [SerializeField] private TextAsset dennisMueller;
    [SerializeField] private TextAsset tv;
    [SerializeField] private TextAsset speakers;
    [SerializeField] private TextAsset records;
    [SerializeField] private TextAsset couch;
    [SerializeField] private TextAsset curtain;
    [SerializeField] private TextAsset roomLight;
    [SerializeField] private TextAsset guitarGear;
    [SerializeField] private TextAsset paintingMR;
    [SerializeField] private TextAsset clockMR;
    [SerializeField] private TextAsset cabinetMR;
    [SerializeField] private TextAsset lavaLampMR;
    [SerializeField] private TextAsset lavaLampSuccess;
    [SerializeField] private TextAsset sheetMusicSuccess;

    [Header("Ink JSON (Art Room)")]
    [SerializeField] private TextAsset cat;
    [SerializeField] private TextAsset henryEntersAR;
    [SerializeField] private TextAsset couchAR;
    [SerializeField] private TextAsset bookshelfAR;
    [SerializeField] private TextAsset fireplaceAR;
    [SerializeField] private TextAsset paintingAR;
    [SerializeField] private TextAsset lampshadeAR;
    [SerializeField] private TextAsset recordPlayerAR;
    [SerializeField] private TextAsset paintBucketAR;
    [SerializeField] private TextAsset easelAR;
    [SerializeField] private TextAsset sketchbookAR;

    [Header("Ink JSON (Hallway)")]
    [SerializeField] private TextAsset passwordCompleteSuccess;
    [SerializeField] private TextAsset passwordBeforeAttempt;
    [SerializeField] private TextAsset henryHeadHurts;

    [Header("Ink JSON (VIP Room)")]
    [SerializeField] private TextAsset vipRoomHenryEnters;
    [SerializeField] private TextAsset vipRoomFoundHenry;

    [Header("Variable Booleans (Pool Room)")]
    public bool vaseSmashed = false;
    public bool gemstoneCollected = false;
    public bool receivedPhoneHint = false;
    public bool phoneDialled = false;
    public bool henryEnterPoolRoomFirst = false;
    public bool heardPaintingQuipPR = false;
    public bool gemstonePuzzleSolved = false;
    public bool phonePuzzleSolved = false;
    public bool heardPoolTableQuip = false;
    public bool heardBookcaseQuip = false;
    public GameObject redBookUIImage;
    public GameObject vase;
    public GameObject gemstone;
    public GameObject gemstoneUIImage;
    public GameObject vaseSmashedSprite;
    public AnimationData lettuceIdleAnimation;
    public AnimationData lettuceCompleteAnimation;

    [Header("Variable Booleans (Hallway)")]
    public bool finishedPoolRoomPuzzles = false;
    public bool finishedArtRoomPuzzles = false;
    public bool finishedMusicRoomPuzzles = false;
    public bool clockSetToTwelve = false;
    public bool enteredPassword = false;
    public bool heardTryMeQuip = false;
    public bool heardHenryHeadHurts = false;
    public GameObject vipRoomUIImage;
    public GameObject enterVIPRoomUIImage;
    public GameObject staircaseCutscene;
    public AnimationData henryStaircaseWalk;
    public GameObject usher;
    public AnimationData usherVanish;
    public GameObject inFrontStaircase;
    public GameObject behindStaircase;

    [Header("Variable Boolean (Art Room)")]
    public bool henryEnterArtRoomFirst = false;
    public bool heardPaintingQuipAR = false;
    public bool heardSketchbookQuipAR = false;
    public bool drawnInNotebook = false;
    public bool solvedSheetMusicPuzzle = false;
    public bool solvedSketchbookPuzzle = false;
    public GameObject doneButtonUIImage;
    public GameObject doneButtonText;
    public GameObject doneButtonOutline;
    public AnimationData meatIdleAnimation;
    public AnimationData meatCompleteAnimation;

    [Header("Variable Boolean (Music Room)")]
    public bool toldToFindPowerCord = false;
    public bool henryEnterMusicRoomFirst = false; 
    public bool heardPaintingQuip = false;
    public bool heardClockQuip = false;
    public bool heardCabinetQuip = false;
    public bool solvedPowerCordPuzzle = false;
    public bool solvedRecordsPuzzle = false;
    public bool solvedLavaLampPuzzle = false;
    public GameObject lavaLampUIImage;
    public GameObject secondaryPianoSheetMusic;
    public GameObject solvedLavaLampPosition;
    public GameObject blueBackground;
    public GameObject emptyTable;
    public GameObject lavaLampOnTable;
    public GameObject bigHand;
    public GameObject smallHand;
    public GameObject clockInMR;
    public GameObject changedClockInMR;
    public GameObject musicRoomDoor;
    public GameObject musicRoomUIImageOpened;
    public GameObject musicRoommUIImageLocked;
    public GameObject hallwayClockAfter12;
    public GameObject edward;
    public AnimationData edwardIdleAnimation;
    public AnimationData pastaIdleAnimation;
    public AnimationData pastaCompleteAnimation;
    
    [Header("Variable Boolean (VIP Room)")]
    public bool watchedEndCutscene = false;
    public GameObject finalCutscene;
    public VideoPlayer videoPlayer;
    public GameObject videoPlayerUIImage;
    private FinalCutscene finalCutscene2;
    private bool hasRun = false;
    
    public void Start()
    {
        redBookUIImage.SetActive(false);
        henryEnterPoolRoomFirst = false;

        pausePanel.SetActive(false);
        isPaused = false;

        finalCutscene2 = FindObjectOfType<FinalCutscene>();

        //sets the initial volume of each soundtrack source
        titleScreenSoundtrack.volume = titleScreenVolume;
        hallwaySoundtrack.volume = hallwayVolume;
        poolRoomSoundtrack.volume = poolRoomVolume;
        artRoomSoundtrack.volume = artRoomVolume;
        musicRoomSoundtrack.volume = musicRoomVolume;

        //sets the initial volume of each record source
        poorlyLitTriangleSoundtrack.volume = poorlyLitVolume;
        oftenTimesSoundtrack.volume = oftenTimesVolume;
        highwayToBellSoundtrack.volume = highwayToBellVolume;
        rillieStylishSoundtrack.volume = rillieStylishVolume;

        //play title screen ambience
        titleScreenSoundtrack.Play();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && activeLocalScene != 0)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                TogglePause();
            }
        }

        if (activeLocalScene == 2)
        {
            FindObjectOfType<GameManager>().CheckPuzzleProgress();
        }

        if (activeLocalScene == 6)
        {
            FindObjectOfType<GameManager>().CheckPuzzleProgress();
        }

        if (activeLocalScene == 7)
        {
            FindObjectOfType<GameManager>().CheckPuzzleProgress();
        }

        if (watchedEndCutscene == true && !hasRun)
        {
            hasRun = true;
            finalCutscene.SetActive(false);
            videoPlayerUIImage.SetActive(false);
            collectedItems.Clear();
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }

        if (!DialogueManager.GetInstance().dialogueIsPlaying && heardHenryHeadHurts == false && activeLocalScene != 0 && activeLocalScene != 16) //&& clickManager.playerWalking == false)
        {
            //1 in 100,000 chance
            int random = Random.Range(0, 100000);
            if (random == 0)
            {
                DialogueManager.GetInstance().EnterDialogueMode(henryHeadHurts);
                heardHenryHeadHurts = true;
            }
        }
    }

    private void Awake()
    {
        Instance = this;

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Time.fixedDeltaTime = 1f / 60f;

        //Force 1920x1080 fullscreen (true = fullscreen)
        Screen.SetResolution(1920, 1080, true);
    }
   
    public void SetHoverCursor()
    {
        Cursor.SetCursor(greenCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetBlockedHoverCursor()
    {
        Cursor.SetCursor(redCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(mainCursor, Vector2.zero, CursorMode.Auto);
    }

    public void TogglePause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ToTitleScreen()
    {
        pausePanel.SetActive(false);
        isPaused = false;
        collectedItems.Clear();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public IEnumerator MoveToPoint(Transform myObject, Vector2 point)
    {
        henryLastPosition = point;
        
        Vector2 positionDifference = point - (Vector2)myObject.position;
        if (myObject.GetComponentInChildren<SpriteRenderer>() && positionDifference.x != 0)
            myObject.GetComponentInChildren<SpriteRenderer>().flipX = positionDifference.x > 0;
        
		while (positionDifference.magnitude > moveAccuracy)
        {
            myObject.Translate(moveSpeed * positionDifference.normalized * Time.deltaTime);
            positionDifference = point - (Vector2)myObject.position;
            yield return null;
        }
        myObject.position = point;

        int currentScene = activeLocalScene;
        henryLastPositionsByScene[currentScene] = point;

        if (myObject == FindObjectOfType<ClickManager>().player)
            FindObjectOfType<ClickManager>().playerWalking = false;
        yield return null;
    }

    public void UpdateNameTag(ItemData item)
    {
        if (item == null)
        {
            nameTag.parent.gameObject.SetActive(false);
            return;
        }
        nameTag.parent.gameObject.SetActive(true);
        string nameText = item.objectName;
        Vector2 size = item.nameTagSize;

        //if we have collected the item, use different name and size
        if (collectedItems.Contains(item))
        {
            nameText = item.itemName;
            size = item.itemnameTagSize;
        }
        //change name
        nameTag.GetComponentInChildren<TextMeshProUGUI>().text = nameText;
        //change size
        nameTag.sizeDelta = size;
        //move tag
        nameTag.localPosition = new Vector2(size.x / 1.50f, -0.175f);
        float xOffset = size.x / 1.50f;
        if (item.flipNameTagSide)
            xOffset *= -1f;

        nameTag.localPosition = new Vector2(xOffset, -0.175f);
    }

    public void UpdateHintBox(ItemData item, bool playerFlipped)
    {
        if (item == null)
        {
            //hide hint box
            hintBox.gameObject.SetActive(false);
            return;
        }
        //show hint box
        hintBox.gameObject.SetActive(true);
        //change text
        hintBox.GetComponentInChildren<TextMeshProUGUI>().text = item.hintMessage;
        //change size
        hintBox.sizeDelta = item.hintBoxSize;
        if (playerFlipped)
            hintBox.parent.localPosition = new Vector2(-1, 0);
        else
            hintBox.parent.localPosition = Vector2.zero;
    }

    public void SelectItem(int equipmentCanvasID)
    {
        //sets all slots to default black transparent background
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].color = defaultSlotColor;
        }

        //Save changes and stop if an empty slot is clicked or the last item is removed
        if(equipmentCanvasID >= collectedItems.Count || equipmentCanvasID < 0)
        {
            //No items selected
            selectedItemID = -1;
            selectedCanvasSlotID = 0;
            return;
        }
        equipmentSlots[equipmentCanvasID].color = selectedItemColor;
        //Save Changes
        selectedCanvasSlotID = equipmentCanvasID;
        selectedItemID = collectedItems[selectedCanvasSlotID].itemID;
    }

    public void UpdateEquipmentCanvas()
    { 
        int itemsAmount = collectedItems.Count, itemSlotsAmount = equipmentSlots.Length;
        
        for(int i = 0; i < itemSlotsAmount; i++)
        {
            //choose between no item image and item sprite
            if (i < itemsAmount && collectedItems[i].itemSlotSprite != null)
                equipmentImages[i].sprite = collectedItems[i].itemSlotSprite;
            else
                equipmentImages[i].sprite = emptyItemSlotSprite;
        }
        //add special conditions for selecting items
        if (itemsAmount == 0)
        {
            SelectItem(-1);
        }
    }

    public void ShowItemName(int equipmentCanvasID)
    {
        //If an item is in this slot
        if (equipmentCanvasID < collectedItems.Count)
            UpdateNameTag(collectedItems[equipmentCanvasID]);
    }

    public void CheckSpecialConditions(ItemData item, bool canGetItem)
    {
        switch (item.itemID)
        {
            // ROOM TRANSITIONS
            case -11:
                //go to the hallway
                StartCoroutine(ChangeScene(1, 0));
                henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = true;
                FadeToTrack(poolRoomSoundtrack, hallwaySoundtrack);
                break;
            case -12:
                //go to pool room
                PlaySound(soundsNames.doorOpen);
                StartCoroutine(ChangeScene(2, 0));
                henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = false;
                FadeToTrack(hallwaySoundtrack, poolRoomSoundtrack);
                break;
            case -13:
                //go to pool table
                StartCoroutine(ChangeScene(5, 0));
                break;
            case -14:
                //go to phone
                StartCoroutine(ChangeScene(3, 0));
                break;
            case -15:
                //go to book shelf
                StartCoroutine(ChangeScene(4, 0));
                break;
            case -16:
                //go to art room after collecting key
                if (canGetItem)
                {
                    PlaySound(soundsNames.unlockingDoor);
                    for (int i = 0; i < collectedItems.Count; i++)
                    {
                        if (collectedItems[i].itemID == 2)
                        {
                            collectedItems[i].itemSlotSprite = completeEquipmentImages[1];
                            SelectItem(-1);
                            break;
                        }  
                    }
                    UpdateEquipmentCanvas();
                    StartCoroutine(ChangeScene(6, 0));
                    FadeToTrack(hallwaySoundtrack, artRoomSoundtrack);
                }
                break;
            case -17:
                //go to music room (door)
                henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = true;
                if (canGetItem)
                {
                    PlaySound(soundsNames.notebookOpen);
                    StartCoroutine(ChangeScene(7, 0));
                    FadeToTrack(hallwaySoundtrack, musicRoomSoundtrack);
                }
                break;
            case -18:
                //go to cabinet (music room)
                StartCoroutine(ChangeScene(8, 0));
                break;
            case -19:
                //go to painting cutaway (music room)
                StartCoroutine(ChangeScene(9, 0));
                break;
            case -20:
                //go to clock cutaway (music room)
                StartCoroutine(ChangeScene(10, 0));
                break;
            case -21:
                //go back to music room
                StartCoroutine(ChangeScene(7, 0));
                FindObjectOfType<GameManager>().CheckPuzzleProgress();
                break;
            case -22:
                //go back to art room
                StartCoroutine(ChangeScene(6, 0));
                Cursor.visible = true;
                    break;
            case -23:
                //go to painting cutaway (art room)
                StartCoroutine(ChangeScene(11, 0));
                break;
            case -24:
                //go to sketchbook cutaway (art room)
                StartCoroutine(ChangeScene(12, 0));
                Cursor.visible = false;
                break;
            case -25:
                //go to drawers cutaway (art room)
                StartCoroutine(ChangeScene(13, 0));
                break;
            case -26:
                //go to piano & table cutaway (music room)
                StartCoroutine(ChangeScene(14, 0));
                break;
            case -27:
                //go to painting cutaway (pool room)
                StartCoroutine(ChangeScene(15, 0));
                break;
            case -28:
                //go to final scene (vip room right hand side)
                isFinalScene = true;
                break;
            case -29:
                {
                    //return to pool room (no sound)
                    StartCoroutine(ChangeScene(2, 0));
                }
                break;
            case -30:
                {
                    StartCoroutine(ChangeScene(1, 0));
                    henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = false;
                    FadeToTrack(artRoomSoundtrack, hallwaySoundtrack);
                }
                break;
            case -32:
                {
                    StartCoroutine(ChangeScene(1, 0));
                    henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = false;
                    FadeToTrack(musicRoomSoundtrack, hallwaySoundtrack);
                }
                break;
            case -33:
                {
                    //go to art room after unlocking (no sound)
                    StartCoroutine(ChangeScene(6, 0));
                    henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = true;
                    FadeToTrack(hallwaySoundtrack, artRoomSoundtrack);
                }
                break;
            case -34:
                {
                    //go to music room after unlocking (no sound)
                    StartCoroutine(ChangeScene(7, 0));
                    henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = true;
                    FadeToTrack(hallwaySoundtrack, musicRoomSoundtrack);
                }
                break;
            // OBSERVATIONAL DIALOGUE (POOL ROOM)
            case 300:
                DialogueManager.GetInstance().EnterDialogueMode(painting);
                break;
            case 301:
                DialogueManager.GetInstance().EnterDialogueMode(poolEquipment);
                break;
            case 302:
                DialogueManager.GetInstance().EnterDialogueMode(chandelier);
                break;
            case 303:
                DialogueManager.GetInstance().EnterDialogueMode(window);
                break;

            // OBSERVATIONAL DIALOGUE (MUSIC ROOM)
            case 304:
                DialogueManager.GetInstance().EnterDialogueMode(dennisMueller);
                break;
            case 305:
                DialogueManager.GetInstance().EnterDialogueMode(tv);
                break;
            case 306:
                DialogueManager.GetInstance().EnterDialogueMode(speakers);
                break;
            case 7:
                DialogueManager.GetInstance().EnterDialogueMode(records);
                break;
            case 308:
                DialogueManager.GetInstance().EnterDialogueMode(couch);
                break;
            case 309:
                DialogueManager.GetInstance().EnterDialogueMode(curtain);
                break;
            case 312:
                DialogueManager.GetInstance().EnterDialogueMode(roomLight);
                break;
            case 313:
                DialogueManager.GetInstance().EnterDialogueMode(guitarGear);
                break;
            // OBSERVATIONAL DIALOGUE (ART ROOM)
            case 314:
                DialogueManager.GetInstance().EnterDialogueMode(cat);
                break;
            case 315:
                DialogueManager.GetInstance().EnterDialogueMode(couchAR);
                break;
            case 316:
                DialogueManager.GetInstance().EnterDialogueMode(bookshelfAR);
                break;
            case 317:
                DialogueManager.GetInstance().EnterDialogueMode(fireplaceAR);
                break;
            case 318:
                henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = true;
                break;  
            case 319:
                DialogueManager.GetInstance().EnterDialogueMode(paintBucketAR);
                break;
            case 320:
                DialogueManager.GetInstance().EnterDialogueMode(easelAR);
                break;
            case 321:
                DialogueManager.GetInstance().EnterDialogueMode(lampshadeAR);
                break;
            // COMPLETED PUZZLES (giving items to characters is in line 73 onwards in ClickManager!)
            case 23:
                phonePuzzleSolved = true;
                break;
            case 6:
                //set lava lamp collected to true? maybe do the same for the power cord to change the dialogue?
                DialogueManager.GetInstance().EnterDialogueMode(lavaLampMR);
                break;
            case 322:
                if (canGetItem & solvedLavaLampPuzzle == false & sheetMusicCutaway == true)
                {
                    PlaySound(soundsNames.lavaLampClonk);
                    sheetMusicCutaway.SetActive(false);
                    blueBackground.SetActive(false);
                    emptyTable.SetActive(false);
                    lavaLampOnTable.SetActive(true);
                    lavaLampCutaway.SetActive(true);
                    solvedLavaLampPosition.SetActive(true);
                    DialogueManager.GetInstance().EnterDialogueMode(lavaLampSuccess);
                    solvedLavaLampPuzzle = true;
                    FindObjectOfType<GameManager>().CheckPuzzleProgress();

                    for (int i = 0; i < collectedItems.Count; i++)
                    {
                        if (collectedItems[i].itemID == 6)
                        {
                            collectedItems[i].itemSlotSprite = completeEquipmentImages[5];
                            SelectItem(-1);
                            break;
                        }
                    }
                }
                break;
            case 323:
                if (secondaryPianoCutaway.activeSelf == true & canGetItem & solvedSheetMusicPuzzle == false)
                {
                    PlaySound(soundsNames.notebookOpen);
                    secondaryPianoCutaway.SetActive(false);
                    blueBackground.SetActive(false);
                    emptyTable.SetActive(false);
                    sheetMusicCutaway.SetActive(true);
                    powerCordGiven.SetActive(false);
                    secondaryPianoSheetMusic.SetActive(true);
                    DialogueManager.GetInstance().EnterDialogueMode(sheetMusicSuccess);
                    lavaLampUIImage.SetActive(true);
                    solvedSheetMusicPuzzle = true;
                    FindObjectOfType<GameManager>().CheckPuzzleProgress();

                    for (int i = 0; i < collectedItems.Count; i++)
                    {
                        if (collectedItems[i].itemID == 4)
                        {
                            collectedItems[i].itemSlotSprite = completeEquipmentImages[3];
                            SelectItem(-1);
                            break;
                        }
                    }
                }
                break;
            case 277:
                henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = false;
                if (finishedArtRoomPuzzles == true & finishedMusicRoomPuzzles == true & finishedPoolRoomPuzzles == true & enteredPassword == false)
                {
                    StartCoroutine(ChangeScene(18, 0));   
                }
                break;
            case 726:
                {
                    henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = true;

                    var henryAnimator = henryTransform.GetComponentInChildren<SpriteAnimator>();

                    henryAnimator.SetBaseAnimation(null);

                    henryAnimator.StopAllCoroutines();

                    //hide henry
                    henry.SetActive(false); 

                    //enable the staircase cutscene
                    staircaseCutscene.SetActive(true);

                    staircaseCutscene.transform.localScale = new Vector3(0.94f, 0.94f, 1f);

                    var cutsceneAnimator = staircaseCutscene.GetComponent<SpriteAnimator>();

                    var usherAnimator = usher.GetComponent<SpriteAnimator>();

                    cutsceneAnimator.onSpecificFrame = (animData, frame) =>
                    {
                        if (frame == 40)
                        {
                            //enable behind staircase
                            behindStaircase.SetActive(true);
                            //disable in front staircase
                            inFrontStaircase.SetActive(false);
                        }
                    };

                    //when animation finishes, load VIP room
                    cutsceneAnimator.onAnimationComplete = (animData) =>
                    {
                        staircaseCutscene.SetActive(false);

                        SpriteRenderer sr = henry.GetComponent<SpriteRenderer>();

                        int currentOrder = sr.sortingOrder;

                        sr.sortingOrder = 1;

                        henry.transform.position = new Vector3(11.2f, 7.80f, 0f);

                        henry.SetActive(true);

                        henryAnimator.PlayAnimation(playerAnimations[0]);

                        StartCoroutine(ChangeScene(16, 0));
                        FadeToTrack(hallwaySoundtrack, vipRoomSoundtrack);
                    };

                    usherAnimator.PlayAnimation(usherVanish);
                    FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
                    cutsceneAnimator.PlayAnimation(henryStaircaseWalk);
                }
                break;
            case 5130:
                {
                    var animator = henryTransform.GetComponentInChildren<SpriteAnimator>();
                    animator.SetBaseAnimation(playerAnimations[4]);

                    DialogueManager.GetInstance().onDialogueExit = () =>
                    {
                        if (finalCutscene2 == null)
                        {
                            finalCutscene2 = FindObjectOfType<FinalCutscene>(true);
                        }
                        // play the video
                        if (finalCutscene2 != null)
                        {
                            finalCutscene2.gameObject.SetActive(true);
                            //Debug.Log("Cutscene playing");
                            finalCutscene2.PlayFinalCutscene();
                            Cursor.visible = false;
                        }
                    };
                    PlaySound(soundsNames.lightsOnVIP);
                    DialogueManager.GetInstance().EnterDialogueMode(vipRoomFoundHenry); 
                }
                break;
            case 6969:
                {
                    PlaySound(soundsNames.notebookOpen);
                    EnableArmCursor(true);
                }
                break;
            case -6969:
                {
                    PlaySound(soundsNames.clockAt12);
                }
                break;
            case 4:
                {
                    PlaySound(soundsNames.drawerOpening);
                }
                break;
            case 0:
                {
                    //remove gemstone after giving it back to crimson   
                    henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = false;
                }
                break;
        }
        //In short, checks to see if the item id == something, then changes scenes.
    }

    public void FadeToTrack(AudioSource fromTrack, AudioSource toTrack)
    {
        StartCoroutine(FadeTracks(fromTrack, toTrack));
    }

    private IEnumerator FadeTracks(AudioSource from, AudioSource to)
    {
        float timeElapsed = 0f;

        float startVolumeFrom = from.volume;
        float startVolumeTo = to.volume;

        while (timeElapsed < fadeTime)
        {
            float t = timeElapsed / fadeTime;

            from.volume = Mathf.Lerp(startVolumeFrom, 0f, t);
            to.volume = Mathf.Lerp(startVolumeTo, 0.15f, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        //making sure of exact values at the end
        from.volume = 0f;
        to.volume = 0.15f;
    }

    public IEnumerator ChangeScene(int sceneNumber, float delay)
    {
        Color c = blockingImage.color;
        int previousScene = activeLocalScene;

        yield return new WaitForSeconds(0.02f);

        //screen goes black (in one second) and clicking is blocked (here is where you'd change the transition type)
        blockingImage.enabled = true;

        while (blockingImage.color.a < 1)
        {
            //increase color.alpha
            c.a += Time.deltaTime;
            yield return null;
            blockingImage.color = c;
        }

        if (activeLocalScene != sceneNumber)
        {
            localScenes[activeLocalScene].SetActive(false);
            localScenes[sceneNumber].SetActive(true);
        }
 
        //start playing soundtrack music as we leave the start scene
        if (activeLocalScene == 0)
        {
            //stop the title screen ambience immediately
            titleScreenSoundtrack.Stop();

            //starts all tracks at once so they sync
            hallwaySoundtrack.Play();
            poolRoomSoundtrack.Play();
            artRoomSoundtrack.Play();
            musicRoomSoundtrack.Play();
            vipRoomSoundtrack.Play();

            //set initial volume
            hallwaySoundtrack.volume = hallwayVolume;
            poolRoomSoundtrack.volume = 0f;
            artRoomSoundtrack.volume = 0f;
            musicRoomSoundtrack.volume = 0f;
            vipRoomSoundtrack.volume = 0f;

            FadeToTrack(titleScreenSoundtrack, hallwaySoundtrack);
        }
           
        //save which one is currently used
        activeLocalScene = sceneNumber;

        //teleport the player
        Vector2 startPos;

        if (henryLastPositionsByScene.TryGetValue(sceneNumber, out startPos))
        {
            FindObjectOfType<ClickManager>().player.position = startPos;
        }
        else
        {
            FindObjectOfType<ClickManager>().player.position = playerStartPositions[sceneNumber].position;
        }

        //show equipment bar
        equipmentCanvas.SetActive(sceneNumber > 0 && sceneNumber < localScenes.Length - 1);

        //play sounds example (use for future reference!)
        //PlaySound(soundsNames.(and then put the variable name here!);

        //show dialogue if neccessary
        if (henryEnterPoolRoomFirst == false && activeLocalScene == 2)
        {
            DialogueManager.GetInstance().EnterDialogueMode(henryEntersPR);
            henryEnterPoolRoomFirst = true;
        }

        if (receivedPhoneHint == true && activeLocalScene == 4)
        {
            DialogueManager.GetInstance().EnterDialogueMode(bookcaseSuccess);
            redBookUIImage.SetActive(true);
        }
        else if (heardBookcaseQuip == false && activeLocalScene == 4)
        {
            DialogueManager.GetInstance().EnterDialogueMode(bookcaseFailure);
            heardBookcaseQuip = true;
        }
        
        if (heardPoolTableQuip == false && activeLocalScene == 5)
        {
            DialogueManager.GetInstance().EnterDialogueMode(poolTable);
            heardPoolTableQuip = true;
        }

        if (henryEnterArtRoomFirst == false && activeLocalScene == 6)
        {
            DialogueManager.GetInstance().EnterDialogueMode(henryEntersAR);
            henryEnterArtRoomFirst = true;
        }

        if (henryEnterMusicRoomFirst == false && activeLocalScene == 7)
        {
            DialogueManager.GetInstance().EnterDialogueMode(henryEntersMR);
            henryEnterMusicRoomFirst = true;
        }

        if (heardCabinetQuip == false && activeLocalScene == 8)
        {
            DialogueManager.GetInstance().EnterDialogueMode(cabinetMR);
            heardCabinetQuip = true;
        }

        if (heardPaintingQuip == false && activeLocalScene == 9)
        {
            DialogueManager.GetInstance().EnterDialogueMode(paintingMR);
            heardPaintingQuip = true;
        }

        if (heardClockQuip == false && activeLocalScene == 10)
        {
            DialogueManager.GetInstance().EnterDialogueMode(clockMR);
            heardClockQuip = true;
        }

        if (heardPaintingQuipAR == false && activeLocalScene == 11)
        {
            DialogueManager.GetInstance().EnterDialogueMode(paintingAR);
            heardPaintingQuipAR = true;
        }

        if (heardSketchbookQuipAR == false && activeLocalScene == 12)
        {
            DialogueManager.GetInstance().EnterDialogueMode(sketchbookAR);
            heardSketchbookQuipAR = true;
        }

        if (heardPaintingQuipPR == false && activeLocalScene == 15)
        {
            DialogueManager.GetInstance().EnterDialogueMode(painting);
            heardPaintingQuipPR = true;
        }

        if (activeLocalScene == 1 & clockSetToTwelve == true & enteredPassword == true)
        {
            DialogueManager.GetInstance().EnterDialogueMode(passwordCompleteSuccess);
            vipRoomUIImage.SetActive(false);
            enterVIPRoomUIImage.SetActive(true);
        }
        if (activeLocalScene == 0 && watchedEndCutscene == true)
        {
            Debug.Log("end game g appears");
            gameCompletionG.SetActive(true);
        }
        if (activeLocalScene == 16)
        {
            henry.SetActive(true);

            SpriteRenderer sr = henry.GetComponent<SpriteRenderer>();

            int currentOrder = sr.sortingOrder;

            sr.sortingOrder = 6;

            DialogueManager.GetInstance().EnterDialogueMode(vipRoomHenryEnters);
            henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = true;

            var animator = henryTransform.GetComponentInChildren<SpriteAnimator>();
            animator.SetBaseAnimation(playerAnimations[2]);
        }
        if (activeLocalScene == 18 && heardTryMeQuip == false)
        {
            DialogueManager.GetInstance().EnterDialogueMode(passwordBeforeAttempt);
            heardTryMeQuip = true;
        }
        //collapsing inventory based on scene
        //sun clock cutaway
        if (activeLocalScene == 10)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

        //pool table cutaway
        if (activeLocalScene == 5)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

        //phone cutaway
        if (activeLocalScene == 3)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

        //painting cutaways
        if (activeLocalScene == 15)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

        if (activeLocalScene == 11)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

        if (activeLocalScene == 9)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

        //sketchbook cutaway
        if (activeLocalScene == 12)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

        //password screen cutaway
        if (activeLocalScene == 18)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

        //vip room failsafe
        if (activeLocalScene == 16)
        {
            FindObjectOfType<InventoryCollapse>().SilentlyCollapseInventory();
        }

            //reset animations
            foreach (SpriteAnimator spriteAnimator in FindObjectsOfType<SpriteAnimator>())
            spriteAnimator.PlayAnimation(null);

        //hide tag name
        UpdateNameTag(null);

        while (blockingImage.color.a > 0)
        {
            //decrease color.alpha
            c.a -= Time.deltaTime;
            yield return null;
            blockingImage.color = c;
        }
        blockingImage.enabled = false;
        yield return null;
    }

    public void PlaySound(soundsNames name)
    {
        if (name != soundsNames.none)
        {
            AudioSource.PlayClipAtPoint(soundEffects[(int)name], transform.position, volume:0.5f);
        }
    }

    public void EnableArmCursor(bool enable)
    {
        if (!armCursor)
        {
            armCursor = FindObjectOfType<ArmCursor>(true);
            if (!armCursor)
            {
                Debug.LogWarning("No ArmCursor Found in Scene");
                return;
            }
        }
        armCursor.SetActive(enable);
    }

    public void CheckPuzzleProgress()
    {
        if (gemstonePuzzleSolved == true & phonePuzzleSolved == true & hasStartedPoolRoomCheck == false & activeLocalScene == 2)
        {
            hasStartedPoolRoomCheck = true;
            finishedPoolRoomPuzzles = true;
            poolRoomPaintingReveal.SetActive(true);
            poolRoomPaintingRevealCutaway.SetActive(true);

            var lettuceAnimator = poolRoomPaintingReveal.GetComponent<SpriteAnimator>();

            lettuceAnimator.PlayAnimation(lettuceCompleteAnimation);

            lettuceAnimator.onAnimationComplete = (animData) =>
            {
                lettuceAnimator.PlayAnimation(lettuceIdleAnimation);
                lettuceAnimator.SetBaseAnimation(lettuceIdleAnimation);
            };
        }

        if (solvedSketchbookPuzzle == true & solvedRecordsPuzzle == true & hasStartedArtRoomCheck == false & activeLocalScene == 6)
        {
            hasStartedArtRoomCheck = true;
            finishedArtRoomPuzzles = true;
            artRoomPaintingReveal.SetActive(true);
            artRoomPaintingRevealCutaway.SetActive(true);

            var meatAnimator = artRoomPaintingReveal.GetComponent<SpriteAnimator>();

            meatAnimator.PlayAnimation(meatCompleteAnimation);

            meatAnimator.onAnimationComplete = (animData) =>
            {
                meatAnimator.PlayAnimation(meatIdleAnimation);
                meatAnimator.SetBaseAnimation(meatIdleAnimation);
            };
        }

        if (solvedPowerCordPuzzle == true & solvedSheetMusicPuzzle == true & solvedLavaLampPuzzle == true & hasStartedMusicRoomCheck == false & activeLocalScene == 7)
        {
            hasStartedMusicRoomCheck = true;
            finishedMusicRoomPuzzles = true;
            musicRoomPaintingReveal.SetActive(true);
            musicRoomPaintingRevealCutaway.SetActive(true);

            var pastaAnimator = musicRoomPaintingReveal.GetComponent<SpriteAnimator>();

            pastaAnimator.PlayAnimation(pastaCompleteAnimation);

            pastaAnimator.onAnimationComplete = (animData) =>
            {
                pastaAnimator.PlayAnimation(pastaIdleAnimation);
                pastaAnimator.SetBaseAnimation(pastaIdleAnimation);
            };
        }
    }

    public void StartGame()
    {
        //Animation.Play("introTransistion");
        PlaySound(soundsNames.startButton);
        StartCoroutine(ChangeScene(1, 5));
        henryTransform = GameObject.FindGameObjectWithTag("Player").transform;
        henryTransform.position = henryLastPosition;
    }

    public void QuitGame()
    {
        PlaySound(soundsNames.phoneButton);
        Application.Quit();
    }
}