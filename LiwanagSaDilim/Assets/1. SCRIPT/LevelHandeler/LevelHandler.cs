using System;
using System.Collections; // BAGO: Kailangan ito para sa IEnumerator (Loading Screen)
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

    // --- BAGO: Mga Settings para sa Loading Screen ---
    [Header("Loading Screen Settings")]
    [SerializeField] GameObject loadingPanel; // I-drag dito ang itim na panel na may Liyab animation
    [SerializeField] float minLoadTime = 1f; // Oras na tatakbo si Liyab (seconds)

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

    // --- MGA BINAGONG BUTTON FUNCTIONS ---
    
    public void RetryLevel()
    {
        Time.timeScale = 1f; // Ibalik sa normal ang oras bago mag-load
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

    // =========================================================
    // --- BAGO: DITO NA GAGAWIN ANG LOADING ANIMATION ---
    // =========================================================
    IEnumerator LoadAsynchronously(string sceneName)
    {
        // 1. I-activate ang Loading Screen
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        float timer = 0f;

        // 2. Simulan ang pag-load sa background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        // Pigilan muna ang Unity na lumipat agad para makita ang animation
        operation.allowSceneActivation = false; 

        // 3. Loop habang naghihintay
        while (!operation.isDone)
        {
            // Bilangin ang oras na lumipas
            timer += Time.unscaledDeltaTime;

            // Ang Unity ay humihinto sa 0.9 progress kapag pinigilan natin lumipat
            if (operation.progress >= 0.9f)
            {
                // Kung tapos na mag-load AT lumampas na sa minimum time (halimbawa: 1.5s)
                if (timer >= minLoadTime)
                {
                    // Lipat na sa next scene!
                    operation.allowSceneActivation = true;
                }
            }

            yield return null; // Maghintay per frame
        }
    }
}