using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectFragments : MonoBehaviour
{
    private LevelHandler levelHandler;

    private void Start()
    {
        levelHandler = FindObjectOfType<LevelHandler>();

        if (levelHandler == null)
        {
            Debug.LogError("ERROR: Walang LevelHandler na mahanap sa Scene");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (levelHandler != null)
            {
                levelHandler.addFragments();
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Nawawala ang LevelHandler");
            }
        }
    }
}