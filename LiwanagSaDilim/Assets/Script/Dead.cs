using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dead : MonoBehaviour
{
    public GameObject Player;
    public bool isDead;

    public GameObject life1;
    public GameObject life2;
    public GameObject life3;
    public GameObject life4;
    public GameObject life5;
    public TMP_Text fireflies;

    private void Start()
    {
        DisableAllLives();
    }

    void Update()
    {
        // --- FIX #1: CLAMPING (Pigilan ang Negative) ---
        // Kung bumaba sa 0, ibalik agad sa 0.
        if (PlayerMovements.lives < 0)
        {
            PlayerMovements.lives = 0;
        }

        // --- FIX #2: TEXT DISPLAY ---
        // Ipakita ang text. Tinanggal ko yung redundant na formatting.
        fireflies.text = PlayerMovements.lives.ToString();


        // --- FIX #3: UI LOGIC (Mas malinis na paraan) ---
        // I-check natin kung ilan ang buhay at i-activate ang tamang object.
        
        if (PlayerMovements.lives >= 5)
        {
            SetActiveLife(life5);
        }
        else if (PlayerMovements.lives == 4)
        {
            SetActiveLife(life4);
        }
        else if (PlayerMovements.lives == 3)
        {
            SetActiveLife(life3);
        }
        else if (PlayerMovements.lives == 2)
        {
            SetActiveLife(life2);
        }
        else if (PlayerMovements.lives == 1)
        {
            SetActiveLife(life1);
        }
        else // Kapag 0 na ang buhay
        {
            DisableAllLives(); // Patayin lahat ng display
        }
    }

    // Helper function para patayin lahat muna bago buksan ang isa
    // Para hindi nagpapatong-patong
    void SetActiveLife(GameObject lifeToActivate)
    {
        DisableAllLives(); // Reset muna
        lifeToActivate.SetActive(true); // Buksan yung kailangan
    }

    void DisableAllLives()
    {
        life1.SetActive(false);
        life2.SetActive(false);
        life3.SetActive(false);
        life4.SetActive(false);
        life5.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isDead = false;
    }
}