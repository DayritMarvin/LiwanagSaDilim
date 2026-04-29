using System.Collections;
using UnityEngine;

public class DelayedFallingBridge : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    [SerializeField] private float startDelay = 1.5f;   // wait before anything happens
    [SerializeField] private float fallDelay = 0.2f;    // small gap after collider off

    private bool activated = false;

    private void Start()
    {
        // 🔒 keep bridge stable at start
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // 🔥 called by TriggerTrap
    public void Activate()
    {
        if (activated)
            return;

        activated = true;
        StartCoroutine(HandleFall());
    }

    private IEnumerator HandleFall()
    {
        // ⏱ WAIT so boulder rolls first
        yield return new WaitForSeconds(startDelay);

        // ❌ remove support (no more standing on it)
        if (col != null)
            col.enabled = false;

        // small delay for dramatic drop
        yield return new WaitForSeconds(fallDelay);

        // ⬇ now it falls
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;
    }
}