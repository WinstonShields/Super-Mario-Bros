using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    private Transform frontCheck;
	private Transform[] topChecks = new Transform[3];
    public Animator animator;
	public bool smashed = false;
	private float deathDelay = 0.2f;

	public AudioClip goombaStompedSound;
	private GameObject player;
	PlayerHealth playerHealth;

  	void Awake()
	{
		topChecks[0] = transform.Find("topCheck");
		topChecks[1] = transform.Find("topFrontCheck");
		topChecks[2] = transform.Find("topBackCheck");

		frontCheck = transform.Find("frontCheck");
        animator = GetComponent<Animator>();

		player = GameObject.Find("Mario");
		playerHealth = (PlayerHealth) player.GetComponent(typeof(PlayerHealth));
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		smashed = isSmashed();

		if (smashed && playerHealth.GetVulnerable()) {
			SmashGoomba();
			StartCoroutine(Die());
		}

        // Create an array of all the colliders in front of the enemy.
		Collider2D[] obstacleHits = Physics2D.OverlapPointAll(frontCheck.position, LayerMask.GetMask("Prop"));
		Collider2D[] enemyHits = Physics2D.OverlapPointAll(frontCheck.position, LayerMask.GetMask("Enemy"));
		Collider2D[] frontHits = new Collider2D[obstacleHits.Length + enemyHits.Length];
		obstacleHits.CopyTo(frontHits, 0);
		enemyHits.CopyTo(frontHits, obstacleHits.Length);

        // Check each of the colliders.
		foreach(Collider2D c in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if(c.tag == "Obstacle" || c.tag == "Enemy")
			{
				// ... Flip the enemy and stop checking the other colliders.
				Flip();
				break;
			}
		}

        // Set the enemy's velocity to moveSpeed in the x direction.
		GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
    }

    public void Flip()
	{
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}


	public bool isSmashed() {
		foreach (Transform check in topChecks) {
			if (Physics2D.Linecast(transform.position, check.position, 1 << LayerMask.NameToLayer("Player"))) {
				return true;
			}
		}

		return false;
	}

	public void SmashGoomba() {
		moveSpeed = 0;
		AudioSource.PlayClipAtPoint(goombaStompedSound, transform.position);
		animator.SetTrigger("Smashed");
	}

	IEnumerator Die() {
		yield return new WaitForSeconds(deathDelay);
		Destroy(gameObject);
	}
}
