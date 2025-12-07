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
        if (PlayerMovements.lives < 0)
        {
            PlayerMovements.lives = 0;
        }

        fireflies.text = PlayerMovements.lives.ToString();
        
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
        else
        {
            DisableAllLives();
        }
    }

    void SetActiveLife(GameObject lifeToActivate)
    {
        DisableAllLives();
        lifeToActivate.SetActive(true);
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