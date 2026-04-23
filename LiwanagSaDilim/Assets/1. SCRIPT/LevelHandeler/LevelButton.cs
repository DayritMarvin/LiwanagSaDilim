using System.Collections; // BAGO: Kailangan ito para sa IEnumerator (Loading)
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
    [SerializeField] GameObject loadingPanel; // I-drag dito ang itim na panel na may Liyab animation
    [SerializeField] float minLoadTime = 1f; // Oras na tatakbo si Liyab (seconds)

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
            // BAGO: Imbes na LoadScene agad, tatawagin natin ang Loading Screen Coroutine
            StartCoroutine(LoadAsynchronously("Level " + level));
        }
    }

    public void Tutorial()
    {
        // BAGO: Tatawagin din natin ang Loading Screen para sa tutorial
        StartCoroutine(LoadAsynchronously("MainStory"));
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