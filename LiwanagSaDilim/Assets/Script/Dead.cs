using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Dead : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dead;
    public GameObject Player;
    public bool isDead;
    public int lives;
   

    void Update()
    {
        if (lives <= 0)
        {
           dead.SetActive(true);
           Player.SetActive(false);
        }

        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
           
            lives -= 1;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isDead = false;
    }
}
