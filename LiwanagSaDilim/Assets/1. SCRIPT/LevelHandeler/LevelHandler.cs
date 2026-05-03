using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] int level = 1;

    [SerializeField] Animator FragmentsPanel1;
    [SerializeField] Animator FragmentsPanel2;
    [SerializeField] Animator FragmentsPanel3;

    [Header("Level Objects")]
    [SerializeField] GameObject exitDoor; 

    [SerializeField] GameObject VictoryPanel;
    [SerializeField] GameObject GameoverPanel;
    [SerializeField] GameObject MenuPanel;

    [Header("Loading Screen Settings")]
    [SerializeField] GameObject loadingPanel;
    [SerializeField] float minLoadTime = 1f;

    [NonSerialized] public int fragmentsCollected = 0;
    public bool levelEnd = false;

    private void Start()
    {
        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Menubtn();
        }
    }

    public void addFragments()
    {
        fragmentsCollected++;
        
        switch (fragmentsCollected)
        {
            case 1:
                FragmentsPanel1.Play("FragmentsPop");
                break;
            case 2:
                FragmentsPanel2.Play("FragmentsPop");
                break;
            case 3:
                FragmentsPanel3.Play("FragmentsPop");
                break;
        }

        if (fragmentsCollected >= 3)
        {
            ShowExitDoor();
        }
    }

    void ShowExitDoor()
    {
        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
        }
    }

    public void levelFinished()
    {
        if (!levelEnd)
        {
            levelEnd = true;
            GameManager.setLevelCollectedFragments(level, fragmentsCollected);
            GameManager.UnlockLevel(level + 1);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.SetActive(false);
            }

            Time.timeScale = 0f;
            VictoryPanel.SetActive(true);
        }
    }

    public void levelFailed()
    {
        if (!levelEnd)
        {
            levelEnd = true;
            GameManager.setLevelCollectedFragments(level, fragmentsCollected);
            Time.timeScale = 0f;
            GameoverPanel.SetActive(true);
        }
    }

    
    public void RetryLevel()
    {
        Time.timeScale = 1f; 
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
    }
    
    public void MainMenu()
    {
        Time.timeScale = 1f; 
        StartCoroutine(LoadAsynchronously("MainMenu"));
    }
    
    public void NextLevel()
    {
        Time.timeScale = 1f; 
        StartCoroutine(LoadAsynchronously("Level " + (level + 1)));
    }
    
    public void Menubtn()
    {
        MenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void Resumebtn()
    {
        MenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        float timer = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
 
        operation.allowSceneActivation = false; 

        while (!operation.isDone)
        {
            timer += Time.unscaledDeltaTime;

            if (operation.progress >= 0.9f)
            {
                if (timer >= minLoadTime)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}