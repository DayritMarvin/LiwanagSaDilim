using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class FallingObject : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f;

    // ✅ ADDED (place with other variables)
    [SerializeField] private float disableDelay = 2f;
 
    private bool falling = false;
 
    [SerializeField] private Rigidbody2D rb;
 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Avoid calling the coroutine multiple times if it's already been called (falling)
        if (falling)
            return; 
 
        // If the player landed on the platform, start falling
        if (collision.transform.tag == "Player")
        {
            StartCoroutine(StartFall());
        }
    }
 
    private IEnumerator StartFall()
    {
        falling = true; 
 
        // Wait for a few seconds before dropping
        yield return new WaitForSeconds(fallDelay);
 
        // Enable rigidbody and destroy after a few seconds
        rb.bodyType = RigidbodyType2D.Dynamic;

        // ✅ ADDED (start disabling after fall)
        StartCoroutine(DisableRigidbody());
    }

    // ✅ ADDED (new coroutine)
    private IEnumerator DisableRigidbody()
    {
        yield return new WaitForSeconds(disableDelay);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}