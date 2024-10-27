using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;


public class Dead : MonoBehaviour
{

    public GameObject dead;
    public GameObject Player;
    public bool isDead;
    public int lives = 5;

    //lives game object

    public GameObject life1;
    public GameObject life2;
    public GameObject life3;
    public GameObject life4;
    public GameObject life5;
    public TMP_Text fireflies;


    //invulnerable
  
    private void Start()
    {
        life5.gameObject.SetActive(false);
        life4.gameObject.SetActive(false);
        life3.gameObject.SetActive(false);
        life2.gameObject.SetActive(false);
        life1.gameObject.SetActive(false);

       
    }

    void Update()
    {
        fireflies.text = PlayerMovements.lives.ToString("Fireflies: " + PlayerMovements.lives);
        if (PlayerMovements.lives <= 5)
        {
            life5.gameObject.SetActive(true);


        }
        if (PlayerMovements.lives <= 4)
        {
            life5.gameObject.SetActive(false);
            life4.gameObject.SetActive(true);
            life3.gameObject.SetActive(false);

        }
        if (PlayerMovements.lives <= 3)
        {
            life4.gameObject.SetActive(false);
            life3.gameObject.SetActive(true);
            life2.gameObject.SetActive(false);

        }
        if (PlayerMovements.lives <= 2)
        {
            life3.gameObject.SetActive(false);
            life2.gameObject.SetActive(true);
            life1.gameObject.SetActive(false);

        }
        if (PlayerMovements.lives <= 1)
        {

            life2.gameObject.SetActive(false);
            life1.gameObject.SetActive(true);


        }
        if (PlayerMovements.lives <= 0)
        {
            dead.SetActive(true);
            Player.SetActive(false);
        }


   
    }
   
    private void OnTriggerExit2D(Collider2D collision)
    {
        isDead = false;
    }
}
