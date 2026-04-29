using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] int level = 1;
    [SerializeField] TextMeshProUGUI levelText;
    
    [SerializeField] GameObject lockGameObject; 
    
    [SerializeField] Image image;
    [SerializeField] Sprite fragmentsCollectedSprite;

    [SerializeField] Image Fragments1;
    [SerializeField] Image Fragments2;
    [SerializeField] Image Fragments3;

    [Header("Loading Screen Settings")]
    [SerializeField] GameObject loadingPanel; 
    [SerializeField] float minLoadTime = 1.5f; 

    bool unlock = false;

    void Start()
    {
        // 1. Tatanungin muna natin ang GameManager
        unlock = GameManager.CheckLevelUnlock(level);

        // =========================================================
        // BAGO: EDITOR OVERRIDE PARA SA MAS MADALING TESTING
        // Kung manu-mano mong tinago (in-uncheck) ang padlock sa Editor,
        // o kaya ay walang nakalagay na padlock, automatic UNLOCKED ito!
        // =========================================================
        if (lockGameObject == null || !lockGameObject.activeSelf)
        {
            unlock = true;
        }

        levelText.text = level.ToString();

        // 2. I-setup ang hitsura at button state
        if (unlock)
        {
            image.color = Color.white; 
            if (lockGameObject != null) lockGameObject.SetActive(false);

            int unlockedHearts = GameManager.CheckLevelFragmentsCollected(level);
            if( unlockedHearts >= 1) Fragments1.sprite = fragmentsCollectedSprite;
            if (unlockedHearts >= 2) Fragments2.sprite = fragmentsCollectedSprite;
            if (unlockedHearts >= 3) Fragments3.sprite = fragmentsCollectedSprite;
        }
        else
        {
            image.color = Color.grey; 
            if (lockGameObject != null) lockGameObject.SetActive(true); 
        }
    }
    
    public void levelButtonPressed()
    {
        if (unlock) 
        {
            StartCoroutine(LoadAsynchronously("Level " + level));
        }
    }

    public void Tutorial()
    {
        StartCoroutine(LoadAsynchronously("MainStory"));
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