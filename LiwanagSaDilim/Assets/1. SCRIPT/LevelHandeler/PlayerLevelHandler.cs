using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerLevelHandler : MonoBehaviour
{
    [NonSerialized] public LevelHandler levelHandler;

    private void Start()
    {
        GameObject[] rootObj = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObj)
        {
            if (levelHandler == null)
            {
                obj.TryGetComponent<LevelHandler>(out levelHandler);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            levelHandler.levelFinished();
        }
        //if (collision.transform.CompareTag("Danger"))
        //{
        //    levelHandler.levelFailed();
        //}
    }

}
