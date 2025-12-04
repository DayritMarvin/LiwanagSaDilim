using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] int level = 1;

    [SerializeField] Animator FragmentsPanel1;
    [SerializeField] Animator FragmentsPanel2;
    [SerializeField] Animator FragmentsPanel3;

    // --- BAGO: Reference para sa Door Object ---
    [Header("Level Objects")]
    [SerializeField] GameObject exitDoor; // I-drag mo dito yung Door object

    [SerializeField] GameObject VictoryPanel;
    [SerializeField] GameObject GameoverPanel;
    [SerializeField] GameObject MenuPanel;

    [NonSerialized] public int fragmentsCollected = 0;
    public bool levelEnd = false;

    private void Start()
    {
        // Siguraduhin na nakatago ang door sa simula ng laro
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
        
        // Play animations based on count
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

        // --- BAGO: Check kung 3 na ang nakuha ---
        if (fragmentsCollected >= 3)
        {
            ShowExitDoor();
        }
    }

    // Function para ilabas ang door
    void ShowExitDoor()
    {
        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
            // Optional: Pwede ka magdagdag ng sound effect dito pag lumabas ang door
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

    // ... (Keep the rest of your button functions: RetryLevel, MainMenu, etc. unchanged) ...
    
    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
    public void NextLevel()
    {
        SceneManager.LoadScene("Level " + (level + 1));
        Time.timeScale = 1f;
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
}