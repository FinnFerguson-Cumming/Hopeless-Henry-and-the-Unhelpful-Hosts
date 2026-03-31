using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowVIPRoom : MonoBehaviour
{
    public float targetX = 18.38f;
    public float moveSpeed = 2f;

    public GameObject buttonUIImage;
    public GameObject buttonAsset;

    GameManager gameManager;

    private bool moveCamera = false;

    // Update is called once per frame
    void Update()
    {
        if (moveCamera)
        {
            buttonAsset.SetActive(false);
            buttonUIImage.SetActive(false);
            //gameManager.SetDefaultCursor();
            Vector3 currentPosition = transform.position;
            float newX = Mathf.MoveTowards(currentPosition.x, targetX, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);

            if (Mathf.Approximately(newX, targetX))
            {
                moveCamera = false;
            }
        }
    }

    public void StartCameraMove()
    {
        moveCamera = true;
    }
}
