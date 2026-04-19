using System.Collections;
using UnityEngine;

public class DisintegratingPlatform : MonoBehaviour
{
    [SerializeField] private float breakDelay = 0.5f;
    [SerializeField] private float respawnDelay = 2f;

    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sr;

    private bool triggered = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered)
            return;

        if (collision.transform.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // ✅ Only trigger from TOP
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

        // ⏱ Delay before disappearing
        yield return new WaitForSeconds(breakDelay);

        // 💥 Disappear
        col.enabled = false;
        sr.enabled = false;

        // ⏳ Wait before respawn
        yield return new WaitForSeconds(respawnDelay);

        // 🔁 Reappear
        col.enabled = true;
        sr.enabled = true;

        triggered = false;
    }
}