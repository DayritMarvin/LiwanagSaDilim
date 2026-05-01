using UnityEngine;

public class Fireflies : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovements player = collision.GetComponent<PlayerMovements>();

            if (player != null)
            {
                player.AddLife();
                Destroy(this.gameObject);
            }
        }
    }
}