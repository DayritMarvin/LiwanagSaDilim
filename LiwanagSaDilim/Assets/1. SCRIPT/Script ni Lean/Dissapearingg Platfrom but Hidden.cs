using System.Collections;
using UnityEngine;

public class DisintegratingPlatformHidden : MonoBehaviour
{
    [SerializeField] private float breakDelay = 0.3f;
    [SerializeField] private float respawnDelay = 2f;

    [SerializeField] private Collider2D col;
    [SerializeField] private GameObject visualChild;
    [SerializeField] private Animator anim;

    private bool triggered = false;
    private bool activePlatform = false;

    private void Start()
    {
        // Start hidden
        if (col != null)
            col.enabled = false;

        if (visualChild != null)
            visualChild.SetActive(false);
    }

    public void Activate()
    {
        if (activePlatform)
            return;

        activePlatform = true;

        if (col != null)
            col.enabled = true;

        if (visualChild != null)
            visualChild.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!activePlatform || triggered)
            return;

        if (collision.transform.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Only trigger if player lands on top
                if (contact.normal.y < -0.5f)
                {
                    StartCoroutine(BreakAndRespawn());
                    break;
                }
            }
        }
    }

    private IEnumerator BreakAndRespawn()
    {
        triggered = true;

        // Play shake animation first
        if (anim != null)
            anim.SetTrigger("Shake");

        yield return new WaitForSeconds(breakDelay);

        // Hide platform
        if (col != null)
            col.enabled = false;

        if (visualChild != null)
            visualChild.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        // Respawn only if activated
        if (activePlatform)
        {
            if (col != null)
                col.enabled = true;

            if (visualChild != null)
                visualChild.SetActive(true);
        }

        triggered = false;
    }
}