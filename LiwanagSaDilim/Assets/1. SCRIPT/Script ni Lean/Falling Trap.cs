using UnityEngine;

public class FallingSpikes : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    private bool activated = false;
    private bool landed = false;

    private void Start()
    {
        if (rb != null)
            rb.gravityScale = 0;
    }

    public void Activate()
    {
        if (activated)
            return;

        activated = true;

        if (rb != null)
            rb.gravityScale = 10;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 🎯 Hit player → destroy
        if (collision.transform.CompareTag("Player"))
        {
            Destroy(gameObject);
            return;
        }

        // 🧱 ONLY trigger landing if coming from above
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f && !landed)
            {
                landed = true;

                if (col != null)
                    col.enabled = false;

                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 0;
                }

                break;
            }
        }
    }
}