using System.Collections;
using UnityEngine;

public class DisintegratingPlatform : MonoBehaviour
{
    [SerializeField] private float breakDelay = 0.3f;
    [SerializeField] private float respawnDelay = 2f;

    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;

    private bool triggered = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered)
            return;

        if (collision.transform.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // only trigger from top
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

        // 🔥 shake first
        if (anim != null)
            anim.SetTrigger("Shake");

        yield return new WaitForSeconds(breakDelay);

        // disappear
        col.enabled = false;
        sr.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // reappear
        col.enabled = true;
        sr.enabled = true;

        triggered = false;
    }
}