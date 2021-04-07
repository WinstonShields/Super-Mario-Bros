using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public bool facingRight = true;
    public bool jump = false;
    public float jumpForce = 10.0f;
    public float bounceForce = 0.3f;
    public float moveForce = 0.03f;
    public float maxSpeed = 1.0f;	
    public float h;
    private Transform[] groundChecks = new Transform[3];
	public bool grounded = false;
    public bool onEnemy = false;
    PlayerHealth playerHealth;
    public int health;

    public AudioClip jumpSound;

    void Awake()
	{
		// Setting up references.
		groundChecks[0] = transform.Find("groundCheck");
        groundChecks[1] = transform.Find("groundFrontCheck");
        groundChecks[2] = transform.Find("groundBackCheck");

        playerHealth = GameObject.FindObjectOfType(typeof(PlayerHealth)) as PlayerHealth;

		animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        health = playerHealth.GetHealth();
        grounded = isGrounded();
        onEnemy = isOnEnemy();

        if (playerHealth.isAlive()) {
            animator.SetTrigger("Alive");

            if (!playerHealth.IsTransforming())
            {
                MovePlayer();
            }

        } else {
            GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * 0, GetComponent<Rigidbody2D>().velocity.y);
        }
    }

    void MovePlayer() {
        
        MoveHorizontal();

        // If the jump button is pressed and the player is grounded then the player should jump.
		if (Input.GetButtonDown("Jump") && grounded)
		{
            // Set the Jump animator trigger parameter.
		    animator.SetTrigger("Jump");
            jump = true;
            Jump();
        }

        if (onEnemy && playerHealth.GetVulnerable()) {
            animator.SetTrigger("Bounce");
            Bounce();
        }
    }

    void MoveHorizontal() {

        h = Input.GetAxis("Horizontal");

        if (grounded) {
            animator.SetTrigger("Grounded");
        }

        // The Speed animator parameter is set to the absolute value of the horizontal input.
		animator.SetFloat("Speed", Mathf.Abs(h));

        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * GetComponent<Rigidbody2D>().velocity.x < maxSpeed)
			// ... add a force to the player.
			GetComponent<Rigidbody2D>().AddForce(Vector2.right * h * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

        if(h > 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && facingRight)
			// ... flip the player.
			Flip();
    }

    void Jump() {

        // Add a vertical force to the player.
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
        AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        jump = false;
    }

    void Bounce() {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, bounceForce));
    }

    void Flip() {
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
    }

    bool isGrounded() {
        foreach (Transform check in groundChecks) {
            if (Physics2D.Linecast(transform.position, check.position, 1 << LayerMask.NameToLayer("Ground")) || 
            Physics2D.Linecast(transform.position, check.position, 1 << LayerMask.NameToLayer("Prop")) ||
            Physics2D.Linecast(transform.position, check.position, 1 << LayerMask.NameToLayer("Hittable"))) {
                return true;
            }
        }

        return false;
    }

    bool isOnEnemy() {
        foreach (Transform check in groundChecks) {
            if (Physics2D.Linecast(transform.position, check.position, 1 << LayerMask.NameToLayer("Enemy"))) {
                return true;
            }
        }
        return false;
    }
}
