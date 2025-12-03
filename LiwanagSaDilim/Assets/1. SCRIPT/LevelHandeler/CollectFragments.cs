using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectFragments : MonoBehaviour
{
    LevelHandler levelHandler;

    private void Start()
    {
        GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjs)
        {
            if(levelHandler == null)
            {
                obj.TryGetComponent<LevelHandler>(out levelHandler);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            levelHandler.addFragments();
            Destroy(gameObject);
        }
    }
}
