using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    bool isTouchingCoin = false;
    public int points = 0;

    private Transform[] topChecks = new Transform[3];

    public int pointsCounter = 0;

    public AudioClip coinSound, oneUpSound;

    PlayerHealth playerHealth;

    void Awake()
    {
        topChecks[0] = transform.Find("topCheck");
		topChecks[1] = transform.Find("topFrontCheck");
		topChecks[2] = transform.Find("topBackCheck");

        playerHealth = (PlayerHealth) GetComponent(typeof(PlayerHealth));
    }

    public int GetPoints() {
        return points;
    }

    public void SetPoints(int p) {
        points = p;
    }

    public int GetPointsCounter()
    {
        return pointsCounter;
    }

    public void SetPointsCounter(int pc)
    {
        pointsCounter = pc;

        if (pointsCounter >= 1000)
        {
            AudioSource.PlayClipAtPoint(oneUpSound, transform.position);
            playerHealth.SetLives(playerHealth.GetLives() + 1);
            pointsCounter = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isTouchingCoin) {
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            points += 100;
            isTouchingCoin = false;

            pointsCounter += 100;

            if (pointsCounter >= 1000)
            {
                AudioSource.PlayClipAtPoint(oneUpSound, transform.position);
                playerHealth.SetLives(playerHealth.GetLives() + 1);
                pointsCounter = 0;
            }
        }

        if (hitBrick()) {
            transform.position = new Vector2(transform.position.x, transform.position.y);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Coin") {
            isTouchingCoin = true;
            Destroy(collider.gameObject);
        }
    }

    public bool hitBrick()
    {
        foreach (Transform check in topChecks) 
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(check.position, LayerMask.GetMask("Hittable"));

            // Check each of the colliders.
            foreach(Collider2D c in hits)
            {
                if(c.tag == "Brick")
                {
                    return true;
                }
            }
        }

        return false;
    }
}
