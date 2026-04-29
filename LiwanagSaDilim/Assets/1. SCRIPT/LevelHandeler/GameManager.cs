using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    // Dictionary para sa fragments (Level Number, Fragment Count)
    static Dictionary<int, int> levelProgress = new Dictionary<int, int>();
    
    // List para sa unlocked levels (True/False)
    static List<bool> levelUnlock = new List<bool>();

    // --- STATIC CONSTRUCTOR (Automatic na tatakbo pag-start ng game) ---
    static GameManager()
    {
        InitializeData(); // Mag-load muna ng defaults
        LoadProgress();   // Tapos i-load ang saved data galing sa cellphone
    }

    // Lagyan ng default values (Level 1 unlocked, iba locked)
    static void InitializeData()
    {
        levelProgress.Clear();
        levelUnlock.Clear();

        // Setup for 10 Levels (Pwede mong dagdagan kung marami kang levels)
        for (int i = 1; i <= 30; i++) 
        {
            levelProgress.Add(i, 0); // Default 0 fragments
            
            if (i == 1) levelUnlock.Add(true); // Level 1 is Unlocked
            else levelUnlock.Add(false);       // Others are Locked
        }
    }

    // --- LOADING LOGIC ---
    static void LoadProgress()
    {
        // I-check isa-isa kung may naka-save na data sa cellphone
        for (int i = 0; i < levelUnlock.Count; i++)
        {
            int levelNum = i + 1;
            string unlockKey = "LevelUnlock_" + levelNum;
            string fragmentKey = "LevelFragments_" + levelNum;

            // 1. Load Unlocks
            if (PlayerPrefs.HasKey(unlockKey))
            {
                // Kung 1 = True, 0 = False
                int status = PlayerPrefs.GetInt(unlockKey);
                levelUnlock[i] = (status == 1);
            }

            // 2. Load Fragments
            if (PlayerPrefs.HasKey(fragmentKey))
            {
                levelProgress[levelNum] = PlayerPrefs.GetInt(fragmentKey);
            }
        }
    }

    // --- PUBLIC FUNCTIONS ---

    public static int CheckLevelFragmentsCollected(int level)
    {
        if (levelProgress.ContainsKey(level))
            return levelProgress[level];
        return 0;
    }

    public static void setLevelCollectedFragments(int level, int collectedFragments)
    {
        if (levelProgress.ContainsKey(level))
        {
            // Update lang kung mas mataas ang nakuha ngayon kesa sa dati
            if (levelProgress[level] < collectedFragments)
            {
                levelProgress[level] = collectedFragments;
                levelProgress[level] = Mathf.Clamp(levelProgress[level], 0, 3);
                
                // SAVE AGAD SA STORAGE
                PlayerPrefs.SetInt("LevelFragments_" + level, levelProgress[level]);
                PlayerPrefs.Save(); 
                
                Debug.Log("Saved Fragments for Level " + level);
            }
        }
    }

    public static void UnlockLevel(int level)
    {
        // Siguraduhing nasa range ang level number
        if (level - 1 < levelUnlock.Count)
        {
            // Kung hindi pa unlocked, i-unlock at i-save
            if (levelUnlock[level - 1] == false)
            {
                levelUnlock[level - 1] = true;

                // SAVE AGAD SA STORAGE
                // PlayerPrefs doesn't support bool, so we use Int (1 = true)
                PlayerPrefs.SetInt("LevelUnlock_" + level, 1);
                PlayerPrefs.Save();

                Debug.Log("Saved Unlock for Level " + level);
            }
        }
    }

    public static bool CheckLevelUnlock(int level)
    {
        if (level - 1 < levelUnlock.Count)
        {
            return levelUnlock[level - 1];
        }
        return false;
    }
    
    // Helper function kung gusto mong i-reset ang game (for testing)
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        InitializeData();
        Debug.Log("Game Progress Reset!");
    }
}