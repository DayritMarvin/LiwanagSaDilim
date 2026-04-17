using UnityEngine;

public class RollingSpikeBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    private bool activated = false;

    public void Activate()
    {
        if (activated)
            return;

        activated = true;

        // 🔥 Enable physics → it will fall
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}