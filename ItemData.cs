using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    [Header("Setup")]
    public Transform goToPoint;
    public int itemID, requiredItemID;
    public string objectName;
    public Vector2 nameTagSize = new Vector2(1.95f, 0.35f);
    public string itemName;
    public Vector2 itemnameTagSize = new Vector2(1.95f, 0.35f);
    public bool flipNameTagSide;

    [Header("Success")]
    public GameObject[] objectsToRemove;
    public GameObject[] objectsToSetActive;
    public GameObject[] objectToSetInActive;
    public Sprite itemSlotSprite;
    public AnimationData successAnimation;

    [Header("Failure")]
    [TextArea(3, 3)]
    public string hintMessage;
    public Vector2 hintBoxSize = new Vector2(6, 0.35f);
}
