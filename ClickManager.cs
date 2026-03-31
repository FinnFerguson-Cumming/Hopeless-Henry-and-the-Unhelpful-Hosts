using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public bool playerWalking;
    public Transform player;
    GameManager gameManager;
    ItemData itemData;

    [SerializeField] private TextAsset gemstoneSuccessJSON;
    [SerializeField] private TextAsset powerCordSuccessJSON;
    [SerializeField] private TextAsset sheetMusicSuccessJSON;
    [SerializeField] private TextAsset recordsSuccessJSON;
    [SerializeField] private TextAsset notebookSuccessJSON;
    [SerializeField] private TextAsset vaseSmashJSON;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        itemData = FindObjectOfType<ItemData>();
    }

    public void GoToItem(ItemData item)
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }
        else
        {
            //show blocking image
            gameManager.blockingImage.enabled = true;

            //update hint box
            gameManager.UpdateHintBox(null, false);

            if (gameManager.activeLocalScene == 16)
            {
                //shadow idle animation
                player.GetComponent<SpriteAnimator>().SetBaseAnimation(gameManager.playerAnimations[2]);
                //shadow walking animation
                player.GetComponent<SpriteAnimator>().PlayAnimation(gameManager.playerAnimations[3]);
            }
            else
            {
                //default idle animation
                player.GetComponent<SpriteAnimator>().SetBaseAnimation(gameManager.playerAnimations[0]);
                //default walking animation
                player.GetComponent<SpriteAnimator>().PlayAnimation(gameManager.playerAnimations[1]);
            }

            playerWalking = true;

            //start moving player
            StartCoroutine(gameManager.MoveToPoint(player, item.goToPoint.position));

            gameManager.blockingImage2.SetActive(true);
            
            TryGettingItem(item);

            //hide blocking image
            gameManager.blockingImage.enabled = false;
        }
    }

    private void TryGettingItem(ItemData item)
    {
        bool canGetItem = item.requiredItemID == -1 || gameManager.selectedItemID == (item.requiredItemID);
        if (canGetItem && item.itemSlotSprite != null)
        {
            gameManager.blockingImage.enabled = true;
            GameManager.collectedItems.Add(item);
            gameManager.PlaySound(GameManager.soundsNames.splashPickup);
            gameManager.nameTag.parent.gameObject.SetActive(false);
        }
        StartCoroutine(UpdateSceneAfterAction(item, canGetItem));
        gameManager.blockingImage.enabled = false;
    }

    private IEnumerator UpdateSceneAfterAction(ItemData item, bool canGetItem)
    {
        while (playerWalking)
            yield return new WaitForSeconds(0.05f);

        if (canGetItem)
        {
            gameManager.blockingImage2.SetActive(false);
            foreach (GameObject g in item.objectsToRemove)
                Destroy(g);
            //show objects
            foreach (GameObject g in item.objectsToSetActive)
                g.SetActive(true);
            foreach (GameObject g in item.objectToSetInActive)
                g.SetActive(false);
            if (item.successAnimation)
            {
                SpriteAnimator animator = item.GetComponent<SpriteAnimator>();
                animator.PlayAnimation(item.successAnimation);
                player.GetComponent<SpriteAnimator>().PlayAnimation(gameManager.playerAnimations[0]);
            }
            
            if (item.itemID == 0)
            {
                DialogueManager.GetInstance().EnterDialogueMode(gemstoneSuccessJSON);
                gameManager.gemstonePuzzleSolved = true;
                FindObjectOfType<GameManager>().CheckPuzzleProgress();
                item.requiredItemID = 10;

                //change gemstone to used version
                for (int i = 0; i < GameManager.collectedItems.Count; i++)
                {
                    if (GameManager.collectedItems[i].itemID == 1)
                    {
                        GameManager.collectedItems[i].itemSlotSprite = gameManager.completeEquipmentImages[0];
                        gameManager.SelectItem(-1);
                        break;
                    }
                }
                gameManager.UpdateEquipmentCanvas();
            }
            if (item.itemID == 80)
            {
                DialogueManager.GetInstance().EnterDialogueMode(powerCordSuccessJSON);

                for (int i = 0; i < GameManager.collectedItems.Count; i++)
                {
                    if (GameManager.collectedItems[i].itemID == 5)
                    {
                        GameManager.collectedItems[i].itemSlotSprite = gameManager.completeEquipmentImages[4];
                        gameManager.SelectItem(-1);
                        break;
                    }
                }

                var edwardAnimator = gameManager.edward.GetComponent<SpriteAnimator>();

                edwardAnimator.onAnimationComplete = (animData) =>
                {
                    gameManager.powerCordGiven.SetActive(true);
                    gameManager.secondaryPianoCutaway.SetActive(true);
                    gameManager.solvedPowerCordPuzzle = true;
                    FindObjectOfType<GameManager>().CheckPuzzleProgress();
                    item.requiredItemID = 10;
                    
                    edwardAnimator.PlayAnimation(gameManager.edwardIdleAnimation);
                };
            }
            if (item.itemID == 81)
            {
                DialogueManager.GetInstance().EnterDialogueMode(sheetMusicSuccessJSON);
                gameManager.solvedSheetMusicPuzzle = true;
                FindObjectOfType<GameManager>().CheckPuzzleProgress();
            }
            if (item.itemID == 318)
            {
                RecordsMenu.Instance.OpenMenu();

                var recordsObject = GameObject.Find("GRAMOPHONE");
                if (recordsObject != null)
                {
                    var itemData = recordsObject.GetComponent<ItemData>();
                    if (itemData != null)
                    {
                        itemData.hintMessage = null;
                        itemData.hintBoxSize = Vector2.zero;
                    }
                }

                for (int i = 0; i < GameManager.collectedItems.Count; i++)
                {
                    if (GameManager.collectedItems[i].itemID == 7)
                    {
                        GameManager.collectedItems[i].itemSlotSprite = gameManager.completeEquipmentImages[6];
                        gameManager.SelectItem(-1);
                        break;
                    }
                }

                gameManager.solvedRecordsPuzzle = true;
                FindObjectOfType<GameManager>().CheckPuzzleProgress();
            }
            if (item.itemID == -6969)
            {
                gameManager.smallHand.transform.Translate(1.7f, 0.58f, 0.5373999f);
                gameManager.smallHand.transform.Rotate(0, 0, -126.719f);
                gameManager.clockSetToTwelve = true;
                gameManager.clockInMR.SetActive(false);
                gameManager.changedClockInMR.SetActive(true);
                gameManager.hallwayClockAfter12.SetActive(true);
            }
            if (item.itemID == 82)
            {
                DialogueManager.GetInstance().EnterDialogueMode(notebookSuccessJSON);
                gameManager.solvedSketchbookPuzzle = true;
                FindObjectOfType<GameManager>().CheckPuzzleProgress();
                item.requiredItemID = 10;

                for (int i = 0; i < GameManager.collectedItems.Count; i++)
                {
                    if (GameManager.collectedItems[i].itemID == 3)
                    {
                        GameManager.collectedItems[i].itemSlotSprite = gameManager.completeEquipmentImages[2];
                        gameManager.SelectItem(-1);
                        break;
                    }
                }
            }
            if (item.itemID == 1)
            {
                gameManager.gemstoneCollected = true;
                PickupSplash.Instance.PlayItemSplash(0);
                PlayHatShake();
            }
            if (item.itemID == 2)
            {
                gameManager.phonePuzzleSolved = true;
                FindObjectOfType<GameManager>().CheckPuzzleProgress();
                PickupSplash.Instance.PlayItemSplash(1);
                PlayHatShake();
            }
            if (item.itemID == 3)
            {
                PickupSplash.Instance.PlayItemSplash(2);
                PlayHatShake();
            }
            if (item.itemID == 4)
            {
                PickupSplash.Instance.PlayItemSplash(3);
                PlayHatShake();
            }
            if (item.itemID == 5)
            {
                PickupSplash.Instance.PlayItemSplash(4);
                PlayHatShake();
            }
            if (item.itemID == 6)
            {
                PickupSplash.Instance.PlayItemSplash(5);
                PlayHatShake();
            }
            if (item.itemID == 7)
            {
                PickupSplash.Instance.PlayItemSplash(6);
                PlayHatShake();
            }
            if (item.itemID == 255)
            {
                gameManager.henryTransform.GetComponentInChildren<SpriteRenderer>().flipX = false;
                gameManager.vaseSmashed = true;
                DialogueManager.GetInstance().EnterDialogueMode(vaseSmashJSON);

                var vaseAnimator = gameManager.vase.GetComponent<SpriteAnimator>();

                vaseAnimator.onNearlyComplete = (animData) =>
                {
                    gameManager.gemstoneUIImage.SetActive(true);
                    gameManager.gemstone.SetActive(true);
                };

                vaseAnimator.onAnimationComplete = (animData) =>
                {
                    var sr = gameManager.vase.GetComponent<SpriteRenderer>();
                    sr.sprite = animData.sprites[animData.sprites.Length - 1];

                    vaseAnimator.StopAllCoroutines();

                    gameManager.gemstoneUIImage.SetActive(true);
                    gameManager.gemstone.SetActive(true);

                    gameManager.vase.SetActive(false);
                    gameManager.vaseSmashedSprite.SetActive(true);
                };

                StopAllCoroutines();
                vaseAnimator.PlayAnimation(item.successAnimation);
            }
        }
        else
        {
            gameManager.UpdateHintBox(item, player.GetComponentInChildren<SpriteRenderer>().flipX);
            gameManager.blockingImage2.SetActive(false);
        }
        gameManager.CheckSpecialConditions(item, canGetItem);
        player.GetComponent<SpriteAnimator>().PlayAnimation(null);
        gameManager.UpdateEquipmentCanvas();
        yield return null;
    }

    private IEnumerator ReturnToIdleAfter(SpriteAnimator animator, AnimationData animation)
    {
        int gap = Mathf.Max(1, animation.frameOfGap);
        float frameRate = 60f / gap;
        float duration = animation.sprites.Length / frameRate;

        yield return new WaitForSeconds(duration);

        //return to idle animation
        animator.PlayAnimation(null);
    }

    public void PlayHatShake()
    {
        var hatAnimator = gameManager.hatSprite.GetComponent<SpriteAnimator>();
        var inventory = FindObjectOfType<InventoryCollapse>();

        if (inventory != null)
        {
            if (inventory.IsCollapsed)
            {
                //hat is right-side up
                hatAnimator.PlayAnimation(gameManager.hatPickupOpenAnimation);
            }
        }
    }
}