using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireflies : MonoBehaviour
{
    
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collides with the item
        if (collision.CompareTag("Player"))
        {
            PlayerMovements.lives += 1;
            Destroy(this.gameObject);
        }
    }
}