using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHit : MonoBehaviour
{
    Transform bottomCheck;
    Vector3 originalPosition, destinationPosition;
    public int coins = 0, movingUp = 0;

    GameObject player;
    PlayerPoints playerPoints;

    public GameObject coin;

    bool ready = true;
    
    public AudioClip brickBumpSound;

    // Start is called before the first frame update
    void Awake()
    {
        // Brick contains a random amount of coins in the range of
        // 0 to 5.
        coins = Random.Range(0, 6);

        bottomCheck = transform.Find("bottomCheck");

        originalPosition = transform.position;
        destinationPosition = new Vector3(transform.position.x, transform.position.y + 0.05f);

        player = GameObject.Find("Mario");
        playerPoints = (PlayerPoints) player.GetComponent(typeof(PlayerPoints));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingUp == 1)
        {
            MoveVertically(1f, destinationPosition);
        }

        if (movingUp == -1)
        {
            MoveVertically(1f, originalPosition);
        }

        if (IsHit() && ready)
        {
            ready = false;

            if (coins > 0)
            {
                // If the brick still has coins, decrement the coins whenever
                // the brick is hit.
                coins -= 1;

                playerPoints.SetPoints(playerPoints.GetPoints() + 200);
                playerPoints.SetPointsCounter(playerPoints.GetPointsCounter() + 200);
                GenerateCoin();
            }
            
            StartCoroutine(Ready());

            movingUp = 1;

            AudioSource.PlayClipAtPoint(brickBumpSound, transform.position);
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

    void GenerateCoin()
    {
        Instantiate(coin, transform.position, Quaternion.identity);
    }

    IEnumerator Ready()
    {
        yield return new WaitForSeconds(0.5f);

        ready = true;
    }
}
