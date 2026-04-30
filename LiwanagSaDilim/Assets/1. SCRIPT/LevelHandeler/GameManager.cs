using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    static Dictionary<int, int> levelProgress = new Dictionary<int, int>();
    static List<bool> levelUnlock = new List<bool>();

    static GameManager()
    {
        InitializeData(); 
        LoadProgress(); // BINALIK NATIN: Para gumana ang saving at unlocking ng Level 6
    }

    static void InitializeData()
    {
        levelProgress.Clear();
        levelUnlock.Clear();

        // Setup para sa 10 Levels 
        for (int i = 1; i <= 10; i++) 
        {
            levelProgress.Add(i, 0); 
            
            // BAGO: I-unlock agad ang Level 1 hanggang 5. I-lock ang 6 pataas.
            if (i <= 1)
            {
                levelUnlock.Add(true); 
            }
            else
            {
                levelUnlock.Add(false);
            }
        }
    }

    static void LoadProgress()
    {
        for (int i = 0; i < levelUnlock.Count; i++)
        {
            int levelNum = i + 1;
            string unlockKey = "LevelUnlock_" + levelNum;
            string fragmentKey = "LevelFragments_" + levelNum;

            if (PlayerPrefs.HasKey(unlockKey))
            {
                int status = PlayerPrefs.GetInt(unlockKey);
                levelUnlock[i] = (status == 1);
            }

            if (PlayerPrefs.HasKey(fragmentKey))
            {
                levelProgress[levelNum] = PlayerPrefs.GetInt(fragmentKey);
            }
        }
    }

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
            if (levelProgress[level] < collectedFragments)
            {
                levelProgress[level] = collectedFragments;
                levelProgress[level] = Mathf.Clamp(levelProgress[level], 0, 3);
                
                PlayerPrefs.SetInt("LevelFragments_" + level, levelProgress[level]);
                PlayerPrefs.Save(); 
            }
        }
    }

    public static void UnlockLevel(int level)
    {
        if (level - 1 < levelUnlock.Count)
        {
            if (levelUnlock[level - 1] == false)
            {
                levelUnlock[level - 1] = true;
                PlayerPrefs.SetInt("LevelUnlock_" + level, 1);
                PlayerPrefs.Save();
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
    
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        InitializeData();
    }
}