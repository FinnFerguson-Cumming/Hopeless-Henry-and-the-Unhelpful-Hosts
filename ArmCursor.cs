using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCursor : MonoBehaviour
{
    [Header("References")]
    public Transform armSprite;
    public Vector2 penTipOffset;

    private bool active = false;

    void Update()
    {
        if (!active) return;

        //get the mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        //move the arm cursor + use the offset
        armSprite.position = mousePos + (Vector3)penTipOffset;
    }

    public void SetActive(bool enable)
    {
        active = enable;

        if (armSprite != null)
        {
            armSprite.gameObject.SetActive(enable);

            if (enable)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                armSprite.position = mousePos + (Vector3)penTipOffset;
            }
        }   

    //hide normal cursor
    Cursor.visible = !enable;
    }
}