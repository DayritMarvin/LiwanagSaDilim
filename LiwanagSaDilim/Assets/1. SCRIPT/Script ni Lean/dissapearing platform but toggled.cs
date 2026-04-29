using System.Collections;
using UnityEngine;

public class DisintegratingPlatformToggle : MonoBehaviour
{
    [SerializeField] private float breakDelay = 0.5f;
    [SerializeField] private float respawnDelay = 2f;

    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sr;

    private bool triggered = false;
    private bool activePlatform = false;

    private void Start()
    {
        // start hidden
        col.enabled = false;
        sr.enabled = false;
    }

    public void Activate()
    {
        activePlatform = true;

        col.enabled = true;
        sr.enabled = true;
    }

    public void Deactivate()
    {
        activePlatform = false;

        col.enabled = false;
        sr.enabled = false;

        StopAllCoroutines();
        triggered = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!activePlatform || triggered)
            return;

        if (collision.transform.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
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

        yield return new WaitForSeconds(breakDelay);

        col.enabled = false;
        sr.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        if (activePlatform)
        {
            col.enabled = true;
            sr.enabled = true;
        }

        triggered = false;
    }
}