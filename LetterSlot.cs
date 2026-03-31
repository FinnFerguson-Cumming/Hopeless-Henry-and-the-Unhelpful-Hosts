using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LetterSlot : MonoBehaviour, IDropHandler
{
    private CheckPassword checkPassword;

    private void Start()
    {
        checkPassword = FindObjectOfType<CheckPassword>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedLetter = eventData.pointerDrag;
        if (droppedLetter == null) return;

        DraggableLetter dragScript = droppedLetter.GetComponent<DraggableLetter>();
        if (dragScript == null) return;

        Transform sourceParent = dragScript.OriginalParent;
        Vector3 sourcePosition = dragScript.OriginalLocalPosition;

        Transform currentChild = transform.childCount > 0 ? transform.GetChild(0) : null;

        if (currentChild != null && currentChild != droppedLetter)
        {
            currentChild.SetParent(sourceParent);
            currentChild.localPosition = sourcePosition;

            DraggableLetter childScript = currentChild.GetComponent<DraggableLetter>();
            if (childScript != null)
            {
                childScript.OriginalParent = sourceParent;
                childScript.OriginalLocalPosition = sourcePosition;
            }
        }

        //set the dragged letter to the slot
        droppedLetter.transform.SetParent(transform);
        droppedLetter.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
  
        dragScript.OriginalParent = transform;
        dragScript.OriginalLocalPosition = Vector3.zero;

        if (checkPassword != null)
        {
            checkPassword.CheckNow();
        }
    }
}
