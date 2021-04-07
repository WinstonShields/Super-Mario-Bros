using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    private Transform frontCheck;
    public AudioClip mushroomAppearsSound;
    Vector3 destinationPosition;

    bool movingUp = true;

    void Awake()
    {
        AudioSource.PlayClipAtPoint(mushroomAppearsSound, transform.position);
        destinationPosition = new Vector3(transform.position.x, transform.position.y + 0.2f);
        frontCheck = transform.Find("frontCheck");
        
        if (movingUp)
        {
            MoveVertically(1f, destinationPosition);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Set the mushroom's velocity to moveSpeed in the x direction.
		GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * moveSpeed, GetComponent<Rigidbody2D>().velocity.y);

        Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, LayerMask.GetMask("Prop"));

         // Check each of the colliders.
		foreach(Collider2D c in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if(c.tag == "Obstacle")
			{
				// ... Flip the enemy and stop checking the other colliders.
				Flip();
				break;
			}
		}
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Coin")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    public void Flip()
	{
		// Multiply the x component of localScale by -1.
		Vector3 mushroomScale = transform.localScale;
		mushroomScale.x *= -1;
		transform.localScale = mushroomScale;
	}

    void MoveVertically(float moveSpeed, Vector3 destinationPosition)
    {
        if (transform.position.y >= destinationPosition.y)
        {
            movingUp = false;
        }

        transform.position = Vector3.MoveTowards(transform.position, destinationPosition, moveSpeed * Time.deltaTime);
    }
}
