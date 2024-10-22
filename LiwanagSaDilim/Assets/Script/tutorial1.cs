using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorial1 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tut;
    public bool isActive;

   
    void Update()
    {
        if (isActive == true)
        {
           tut.SetActive(true);
        }
        else
        {
            tut.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isActive = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isActive = false;
    }
}
