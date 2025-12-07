using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    static Dictionary<int,int> levelProgress = new Dictionary<int, int>()
    {

        {1,0},
//       ^level number
//         ^Heart Count
        {2,0},
        {3,0},
        {4,0},
        {5,0},
        {6,0},
        {7,0},
        {8,0},
        {9,0},
        {10,0},

    };

    static List<bool> levelUnlock = new List<bool>()
    {
        true,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
    };

    public static int CheckLevelFragmentsCollected(int level)
    {
        return levelProgress[level];
    }

    public static void setLevelCollectedFragments(int  level, int collectedFragments)
    {
        if (levelProgress[level] < collectedFragments)
        {
            levelProgress[level] = collectedFragments;
            levelProgress[level] = Mathf.Clamp(levelProgress[level], 0, 3);
            Debug.Log("Total Collected Fragments in level " + level + ": " + levelProgress[level] + " out of 3");
        }
    }

    public static void UnlockLevel(int level)
    {
        levelUnlock[level - 1] = true;
    }

    public static bool CheckLevelUnlock(int level)
    {
        return levelUnlock[level - 1];
    }

    public static void printAllUnlockLevels()
    {
        for (int i = 1; i < levelUnlock.Count; i++) 
        {
            Debug.Log("Level " + i + ": " + levelUnlock[i-1]);
        }
    }
}
