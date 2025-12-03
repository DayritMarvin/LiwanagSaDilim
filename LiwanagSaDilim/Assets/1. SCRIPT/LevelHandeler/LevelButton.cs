using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] int level = 1;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject lockGameObject;
    [SerializeField] Image image;

    [SerializeField] Sprite fragmentsCollectedSprite;

    [SerializeField] Image Fragments1;
    [SerializeField] Image Fragments2;
    [SerializeField] Image Fragments3;

    bool unlock = false;

    void Start()
    {
        unlock = GameManager.CheckLevelUnlock(level);

        levelText.text = level.ToString();

        if (unlock)
        {
            image.color = Color.white;
            lockGameObject.SetActive(false);

            int unlockedHearts = GameManager.CheckLevelFragmentsCollected(level);

            if( unlockedHearts >= 1)
            {
                Fragments1.sprite = fragmentsCollectedSprite;
            }
            if (unlockedHearts >= 2)
            {
                Fragments2.sprite = fragmentsCollectedSprite;
            }
            if (unlockedHearts >= 3)
            {
                Fragments3.sprite = fragmentsCollectedSprite;
            }
        }
        else
        {
            image.color = Color.grey;
            lockGameObject.SetActive(true);
        }

    }
    
    public void levelButtonPressed()
    {
        if (unlock)
        {
            SceneManager.LoadScene("Level "+ level);
        }
    }

    public void Testing()
    {
        SceneManager.LoadScene("LevelHandler");
    }
}
