using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseQuestionBlock : MonoBehaviour
{
    Transform bottomCheck;
    Vector2 originalPosition, destinationPosition;

    Animator animator;

    public GameObject[] items;

    GameObject player;
    PlayerPoints playerPoints;

    int movingUp = 0;

    public bool used = false;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Mario");
        playerPoints = (PlayerPoints) player.GetComponent(typeof(PlayerPoints));

        bottomCheck = transform.Find("bottomCheck");
        originalPosition = transform.position;
        destinationPosition = new Vector3(transform.position.x, transform.position.y + 0.05f);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetBool("Used", used);

        if (movingUp == 1)
        {
            MoveVertically(1f, destinationPosition);
        }

        if (movingUp == -1)
        {
            MoveVertically(1f, originalPosition);
        }

        if (IsHit() && !used)
        {
            used = true;
            movingUp = 1;
            GenerateItem();
        }

    }

    public int IsMovingUp()
    {
        return movingUp;
    }

    bool IsHit() 
    {
        if (Physics2D.Linecast(transform.position, bottomCheck.position, 1 << LayerMask.NameToLayer("Player")))
        {
            return true;
        }

        return false;
    }

   void MoveVertically(float moveSpeed, Vector3 destinationPosition)
    {
        if (movingUp == 1 && transform.position.y >= destinationPosition.y)
        {
            movingUp = -1;
        }

        transform.position = Vector3.MoveTowards(transform.position, destinationPosition, moveSpeed * Time.deltaTime);
    }

    void GenerateItem()
    {
        int itemIndex = Random.Range(0, items.Length);

        Instantiate(items[itemIndex], transform.position, Quaternion.identity);

        if (itemIndex == 0)
        {
            playerPoints.SetPoints(playerPoints.GetPoints() + 200);
            playerPoints.SetPointsCounter(playerPoints.GetPointsCounter() + 200);
        }
    }
}
