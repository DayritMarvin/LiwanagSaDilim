using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class SimpleUIFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("Check this for UI Panels (Settings, Menu, etc.) that will pop up.")]
    public bool fadeInOnEnable = true; 
    
    [Tooltip("Check this for the Black Screen when a scene starts.")]
    public bool fadeInOnSceneStart = false; 
    
    [Header("Duration (in seconds)")]
    [Tooltip("How long it takes to appear.")]
    public float fadeInDuration = 1f;
    
    [Tooltip("How long it takes to disappear.")]
    public float fadeOutDuration = 1f;

    private CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    // --- FADE IN LOGIC ---
    void OnEnable()
    {
        if (fadeInOnEnable && !fadeInOnSceneStart)
        {
            cg.alpha = 0f; 
            StartCoroutine(FadeInPanel());
        }
    }

    void Start()
    {
        if (fadeInOnSceneStart)
        {
            cg.alpha = 1f; 
            cg.blocksRaycasts = true; // Prevent clicks while fading in
            StartCoroutine(FadeInScene());
        }
    }

    IEnumerator FadeInPanel()
    {
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    IEnumerator FadeInScene()
    {
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            // From Black (1) to Clear (0)
            cg.alpha = Mathf.Lerp(1f, 0f, timer / fadeInDuration);
            yield return null;
        }
        cg.alpha = 0f;
        cg.blocksRaycasts = false; // Allow clicks again
    }

    // =========================================================
    // --- FADE OUT LOGIC ---
    // =========================================================

    // 1. FOR PANELS
    public void ClosePanel()
    {
        StartCoroutine(FadeOutAndClose());
    }

    IEnumerator FadeOutAndClose()
    {
        cg.blocksRaycasts = true; 
        float timer = 0f;
        
        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            // From Visible (1) to Invisible (0)
            cg.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration); 
            yield return null;
        }
        
        cg.alpha = 0f;
        gameObject.SetActive(false); 
    }

    // 2. FOR SCENE TRANSITION
    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        cg.blocksRaycasts = true;
        float timer = 0f;
        
        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            // From Clear (0) to Black (1)
            cg.alpha = Mathf.Lerp(0f, 1f, timer / fadeOutDuration); 
            yield return null;
        }
        
        cg.alpha = 1f;
        Time.timeScale = 1f; 
        SceneManager.LoadScene(sceneName); 
    }
}