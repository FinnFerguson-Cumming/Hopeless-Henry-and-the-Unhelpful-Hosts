using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public GameManager gameManager;
    public float speed = 10f;

    public float movementSpeed = 0f;

    public Vector2 lastClickPos;

    public Vector3 mousePos;

    bool moving;

    private void Update()
    {
        Movement();
    }

    private void OnEnable()
    {
        //remember this it might fix stuff in the future
        gameManager = FindObjectOfType<GameManager>();
    }

    void Movement()
    {
        animator.SetFloat("Speed", Mathf.Abs(movementSpeed));

        if (Input.GetMouseButtonDown(0))
        {
            lastClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //saving henry's position
            //gameManager.henryLastPosition.position = new Vector3(lastClickPos.x, lastClickPos.y, 0);
            //printing out henry's position
            Debug.Log(lastClickPos);
            moving = true;
            mousePos = Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            FlipSprite();
            movementSpeed = speed;
        }

        if (moving && (Vector2)transform.position != lastClickPos)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, lastClickPos, step);
        }
        else
        {
            moving = false;
            movementSpeed = 0f;
        }
    }

    void FlipSprite()
    {
        if (mousePos.x < 0)
        {
            gameObject.transform.localScale = new Vector3(-0.2752805f, 0.2752805f, 0.2752805f);
        }
        else
            gameObject.transform.localScale = new Vector3(0.2752805f, 0.2752805f, 0.2752805f);
    }
}