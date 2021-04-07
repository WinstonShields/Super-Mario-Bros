using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health = 1;
    public int lives = 3;

    public float jumpForce = 6.0f;

    public Animator animator;

    private Transform groundCheck;
    private Transform[] sideChecks = new Transform[2];
    private Transform[] topChecks = new Transform[3];

    public bool vulnerable = true, isTransforming = false;
    private float vulnerabilityTime = 0.2f;
    private int numBlinks = 10;
    private float transformTime = 1f;
    private float pauseTime = 0.7f;
    
    GameObject mainCamera;
    CameraFollow cameraFollow;
    public int points = 0;

    public AudioClip deathSound, oneUp, powerUp, powerDown;

    PlayerPoints playerPoints;

    void Awake()
	{
		sideChecks[0] = transform.Find("frontCheck");
        sideChecks[1] = transform.Find("backCheck");

        topChecks[0] = transform.Find("topCheck");
		topChecks[1] = transform.Find("topFrontCheck");
		topChecks[2] = transform.Find("topBackCheck");

        groundCheck = transform.Find("groundCheck");

        animator = GetComponent<Animator>();

        mainCamera = GameObject.Find("Main Camera");
        cameraFollow = (CameraFollow) mainCamera.GetComponent(typeof(CameraFollow));

        playerPoints = GameObject.FindObjectOfType(typeof(PlayerPoints)) as PlayerPoints;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetInteger("Health", health);

        if (vulnerable)
        {
            if (isHitByEnemy() && health == 1) {
                health -= 1;
                StartCoroutine(Die());
            }

            else if (isHitByEnemy() && health > 1) {
                vulnerable = false;
                StartCoroutine(PowerDown());
                health -= 1;
            }
        } else {

        }

        points = playerPoints.GetPoints();
    }

    public int GetHealth() {
        return health;
    }

    public int GetLives() {
        return lives;
    }

    public void SetLives(int l) {
        lives = l;
    }

    public bool GetVulnerable()
    {
        return vulnerable;
    }

    public bool isAlive() {
        return health >= 1;
    }

    public bool IsTransforming()
    {
        return isTransforming;
    }

    public bool isHitByEnemy() {

        Transform[] checks = new Transform[sideChecks.Length + topChecks.Length];
        sideChecks.CopyTo(checks, 0);
		topChecks.CopyTo(checks, sideChecks.Length);

        foreach (Transform check in checks)
        {
            // Create an array of all the colliders in front of the player.
            Collider2D[] hits = Physics2D.OverlapPointAll(check.position, LayerMask.GetMask("Enemy"));

            // Check each of the colliders.
            foreach(Collider2D c in hits)
            {
                // If any of the colliders is an Obstacle...
                if(c.tag == "Enemy")
                {
                    return true;
                }
            }
        }

        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Mushroom")
        {
            Destroy(collision.gameObject);
            
            if (health < 2) {
                health += 1;

                vulnerable = false;
                isTransforming = true;
                StartCoroutine(PowerUp());
            }
        }

        if (collision.gameObject.tag == "Enemy" && !vulnerable)
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    IEnumerator Die()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
        animator.SetTrigger("Death");

        yield return new WaitForSeconds(pauseTime);

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        GetComponent<BoxCollider2D> ().enabled = false;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
        cameraFollow.FreezeCamera();
    }

    IEnumerator InvulnerablePeriod()
    {
        for (int i = 0; i < numBlinks; i++)
        {
            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
            yield return new WaitForSeconds(vulnerabilityTime);
        }

        vulnerable = true;
        GetComponent<Renderer>().enabled = true;
    }

    IEnumerator PowerUp()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(0.16f, 0.30f);
        groundCheck.position = new Vector3(groundCheck.position.x, groundCheck.position.y - 0.1f, groundCheck.position.z);

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        animator.SetTrigger("Power Up");

        AudioSource.PlayClipAtPoint(powerUp, transform.position);

        yield return new WaitForSeconds(transformTime);

        vulnerable = true;

        isTransforming = false;

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator PowerDown()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(0.14f, 0.16f);

        animator.SetTrigger("Power Up");

        AudioSource.PlayClipAtPoint(powerDown, transform.position);

        yield return new WaitForSeconds(transformTime);

        isTransforming = false;

        yield return StartCoroutine(InvulnerablePeriod());
    }

}
