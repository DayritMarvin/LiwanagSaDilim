using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class SimpleFadeIn : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration = 2.0f;
    public float delayBeforeFade = 0.5f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 1f; 
    }

    void Start()
    {
        StartCoroutine(StartFadeIn());
    }

    IEnumerator StartFadeIn()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false; 
    }
}