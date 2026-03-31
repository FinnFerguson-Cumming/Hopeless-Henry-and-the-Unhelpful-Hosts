using System.Collections;
using UnityEngine;

public class InventoryCollapse : MonoBehaviour
{
    [Header("Slot Settings")]
    [SerializeField] private RectTransform[] slotRects;
    [SerializeField] private float slotSpacing = 80f;
    [SerializeField] private float slideDuration = 0.2f;
    [SerializeField] private float staggerDelay = 0.10f;

    [Header("Button Reference")]
    [SerializeField] private RectTransform buttonRect;

    private bool isCollapsed = true;
    private Vector2 collapsedPosition;
    private Vector2[] expandedPositions;

    GameManager gameManager;

    void Start()
    {
        //calling game manager
        gameManager = FindObjectOfType<GameManager>();

        //collapsed position, the hat button's anchored position
        collapsedPosition = buttonRect.anchoredPosition;

        expandedPositions = new Vector2[slotRects.Length];

        //pre-calculate expanded positions in a neat line
        for (int i = 0; i < slotRects.Length; i++)
        {
            expandedPositions[i] = collapsedPosition + new Vector2((i + 1) * slotSpacing, 0f);
        }

        //initialize all slots to collapsed position
        foreach (var slot in slotRects)
        {
            slot.anchoredPosition = collapsedPosition;
        }
    }

    public void ToggleInventory()
    {
        isCollapsed = !isCollapsed;
        StopAllCoroutines();
        gameManager.PlaySound(GameManager.soundsNames.inventoryOpen);
        StartCoroutine(SlideSlots());
    }

    //collapse inventory from other scripts - FindObjectOfType<InventoryCollapse().CollapseInventory();
    public void CollapseInventory()
    {
        if (!isCollapsed)
        {
            isCollapsed = true;
            StopAllCoroutines();
            gameManager.PlaySound(GameManager.soundsNames.inventoryOpen);
            StartCoroutine(SlideSlots());
        }
    }

    public void SilentlyCollapseInventory()
    {
        if (!isCollapsed)
        {
            isCollapsed = true;
            StopAllCoroutines();
            StartCoroutine(SlideSlots());
        }
    }

    //expand inventory from other scripts - FindObjectOfType<InventoryCollapse().ExpandInventory();
    public void ExpandInventory()
    {
        if (isCollapsed)
        {
            isCollapsed = false;
            StopAllCoroutines();
            gameManager.PlaySound(GameManager.soundsNames.inventoryOpen);
            StartCoroutine(SlideSlots());
        }
    }

    private IEnumerator SlideSlots()
    {
        int count = slotRects.Length;

        //expand from last to first, and collapse from first to last
        int start = isCollapsed ? count - 1 : 0;
        int end = isCollapsed ? -1 : count;
        int step = isCollapsed ? -1 : 1;

        for (int i = start; i != end; i += step)
        {
            Vector2 target = isCollapsed ? collapsedPosition : expandedPositions[i];
            StartCoroutine(SlideSlot(slotRects[i], target));
            yield return new WaitForSeconds(staggerDelay);
        }

        var hatAnimator = gameManager.hatSprite.GetComponent<SpriteAnimator>();

        if (isCollapsed)
        {
            hatAnimator.PlayAnimation(gameManager.hatOpenForwards);
        }
        else
        {
            hatAnimator.PlayAnimation(gameManager.hatOpenBackwards);
        }
    }

    private IEnumerator SlideSlot(RectTransform slot, Vector2 target)
    {
        Vector2 start = slot.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            slot.anchoredPosition = Vector2.Lerp(start, target, elapsed / slideDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        slot.anchoredPosition = target;
    }

    public bool IsCollapsed => isCollapsed;
}