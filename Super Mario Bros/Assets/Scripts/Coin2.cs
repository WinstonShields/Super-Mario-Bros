using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin2 : MonoBehaviour
{
    public AudioClip coinSound;
    Animator animator;
    Vector3 originalPosition, destinationPosition;
    public float timeDelay = 5f;
    bool movingUp = true;

    void Awake()
    {
        AudioSource.PlayClipAtPoint(coinSound, transform.position);
        animator = GetComponent<Animator>();
        originalPosition = transform.position;
        destinationPosition = new Vector3(transform.position.x, transform.position.y + 0.7f);
    }

    void FixedUpdate()
    {
        CoinAnimation();
        StartCoroutine(Remove());
    }

    void CoinAnimation()
    {

        // StartCoroutine(MoveUp(20f, 20f, 3f));

        if (movingUp)
        {
            MoveVertically(2f, destinationPosition);
        }

        else
        {
            MoveVertically(2f, originalPosition);
        }


        animator.SetTrigger("Spin");
    }

    void MoveVertically(float moveSpeed, Vector3 destinationPosition)
    {
        if (transform.position.y >= destinationPosition.y)
        {
            movingUp = false;
        }

        transform.position = Vector3.MoveTowards(transform.position, destinationPosition, moveSpeed * Time.deltaTime);
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(timeDelay);
        Destroy(gameObject);
    }
}
