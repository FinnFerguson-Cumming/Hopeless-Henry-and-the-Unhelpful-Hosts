using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableLetter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public Transform OriginalParent { get; set; }
    public Vector3 OriginalLocalPosition { get; set; }
    public Quaternion OriginalRotation { get; set; }

    private Transform dragLayer;

    private void Awake()
    {
        //canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        var rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        dragLayer = rootCanvas.transform.Find("DragLayer");

        OriginalLocalPosition = rectTransform.localPosition;
        OriginalRotation = rectTransform.localRotation;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //save original parent, local position and rotation
        OriginalParent = transform.parent;
        OriginalLocalPosition = rectTransform.localPosition;
        OriginalRotation = rectTransform.localRotation;

        canvasGroup.blocksRaycasts = false;

        //move to DragLayer instead of default Canvas
        if (dragLayer != null)
        {
            transform.SetParent(dragLayer, false);
            transform.SetAsLastSibling();
        }
        else
        {
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.parent,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
            );

        rectTransform.localPosition = localPoint;
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == dragLayer)
        {
            transform.SetParent(OriginalParent);
            rectTransform.localPosition = OriginalLocalPosition;
            rectTransform.localRotation = OriginalRotation;
        }   
    }
}