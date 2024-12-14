using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireflies : MonoBehaviour
{
    [SerializeField]private bool heal = false;
    public GameObject effectCanvas;


    private void Update()
    {
        if (heal == true)
        {
            StartCoroutine("Healing");
            heal = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collides with the item
        if (collision.CompareTag("Player"))
        {
            heal = true;
            PlayerMovements.lives += 1;
            Destroy(this.gameObject);
          
           
        }
    }
    IEnumerator Healing()
    {
        effectCanvas.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        effectCanvas.SetActive(false);

    }
}