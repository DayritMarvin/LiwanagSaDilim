using UnityEngine;

public class ShowGroupOnTrigger : MonoBehaviour
{
    private Collider2D[] colliders;
    private SpriteRenderer[] renderers;

    private void Start()
    {
        // 🔒 get all children components
        colliders = GetComponentsInChildren<Collider2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();

        // 🔒 hide everything
        foreach (Collider2D c in colliders)
            c.enabled = false;

        foreach (SpriteRenderer r in renderers)
            r.enabled = false;
    }

    // 🔥 called by TriggerTrap
    public void Activate()
    {
        foreach (Collider2D c in colliders)
            c.enabled = true;

        foreach (SpriteRenderer r in renderers)
            r.enabled = true;
    }
}