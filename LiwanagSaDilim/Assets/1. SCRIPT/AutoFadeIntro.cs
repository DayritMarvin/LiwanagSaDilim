using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Kailangan para makalipat ng scene

public class AutoFadeIntro : MonoBehaviour
{
    [Header("Settings")]
    public CanvasGroup uiCanvasGroup; // Dito ida-drag ang Canvas
    public float fadeDuration = 1.5f; // Gaano katagal ang fade-in/out (seconds)
    public float stayDuration = 3f;   // Gaano katagal babasahin ang text (seconds)
    public string nextSceneName = "MainMenu"; // Pangalan ng sunod na scene

    void Start()
    {
        // Siguraduhing invisible sa simula
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            // Simulan ang sequence
            StartCoroutine(IntroSequence());
        }
    }

    // Ito ang "Recipe" ng sunod-sunod na gagawin
    IEnumerator IntroSequence()
    {
        // 1. Wait ng konti bago magsimula (Optional)
        yield return new WaitForSeconds(0.5f);

        // 2. FADE IN (Mula 0 papuntang 1)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Gamit ang Lerp para smooth ang transition
            uiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null; // Hintay ng next frame
        }
        uiCanvasGroup.alpha = 1f; // Siguraduhing buo ang kulay sa dulo

        // 3. STAY (Magpakita ng ilang segundo para mabasa)
        yield return new WaitForSeconds(stayDuration);

        // 4. FADE OUT (Mula 1 papuntang 0)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            uiCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        uiCanvasGroup.alpha = 0f; // Siguraduhing invisible sa dulo

        // 5. Wait lang saglit bago lumipat
        yield return new WaitForSeconds(0.5f);

        // 6. Load na ang Main Menu
        SceneManager.LoadScene(nextSceneName);
    }
}