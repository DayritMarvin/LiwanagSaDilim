using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoFadeIntro : MonoBehaviour
{
    [Header("Settings")]
    public CanvasGroup uiCanvasGroup; 
    public float fadeDuration = 1.5f; 
    public string nextSceneName = "MainMenu"; 

    // BAGO: Variable para ma-check kung pwede na bang pumindot ang player
    private bool canTap = false;

    void Start()
    {
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            StartCoroutine(FadeInSequence());
        }
    }

    void Update()
    {
        // Kapag tapos na ang fade in (canTap = true) AT pumindot ang player (Screen tap o Mouse click)
        if (canTap && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            canTap = false; // Disable agad para hindi mag-doble tap
            StartCoroutine(FadeOutAndLoad());
        }
    }

    IEnumerator FadeInSequence()
    {
        yield return new WaitForSeconds(0.5f);

        // FADE IN (Mula 0 papuntang 1)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            uiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        uiCanvasGroup.alpha = 1f; 

        // Pwede na pumindot ang player!
        canTap = true;
    }

    IEnumerator FadeOutAndLoad()
    {
        // FADE OUT (Mula 1 papuntang 0)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            uiCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        uiCanvasGroup.alpha = 0f; 

        yield return new WaitForSeconds(0.5f);

        // Load na ang Main Menu
        SceneManager.LoadScene(nextSceneName);
    }
}