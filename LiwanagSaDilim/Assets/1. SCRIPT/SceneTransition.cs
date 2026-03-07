using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
            return; 
        }

        // IMPORTANTE: Pag-load pa lang ng scene, gawin na agad solid black
        // Para walang "flash" ng environment bago mag-fade in.
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f; 
        }
    }

    void Start()
    {
        // 1. Kapag papasok sa scene: Mula Black (1) papuntang Malinaw (0)
        if (fadeCanvasGroup != null)
        {
            StartCoroutine(FadeInScene());
        }
    }

    // Ito ang tatawagin mo sa mga buttons o portals mo
    public void LoadNextScene(string sceneName)
    {
        // 2. Kapag palabas ng scene: Magfe-fade out muna, bago mag-load
        StartCoroutine(FadeOutScene(sceneName));
    }

    IEnumerator FadeInScene()
    {
        fadeCanvasGroup.blocksRaycasts = true; // Bawal pumindot habang nagta-transition
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp mula 1 (Black) papuntang 0 (Clear)
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration); 
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f; // Malinaw na ang screen
        fadeCanvasGroup.blocksRaycasts = false; // Pwede na mag-laro
    }

    IEnumerator FadeOutScene(string sceneName)
    {
        fadeCanvasGroup.blocksRaycasts = true; 
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp mula 0 (Clear) papuntang 1 (Black)
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration); 
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f; // Solid Black na
        
        // 3. Dahil black na ang screen, ligtas na tayong lumipat ng scene
        SceneManager.LoadScene(sceneName); 
    }
}