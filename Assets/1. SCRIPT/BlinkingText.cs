using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BlinkingText : MonoBehaviour
{
    [Header("Blink Settings")]
    public float speed = 1.5f;
    public float minFade = 0.2f;
    public float maxFade = 1f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        float pulse = Mathf.PingPong(Time.time * speed, 1f);

        canvasGroup.alpha = Mathf.Lerp(minFade, maxFade, pulse);
    }
}