using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaleAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    [Header("Scale Settings")]
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);
    public Vector3 highlightScale = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);

    [Header("Animation Speed")]
    public float animSpeed = 15f;

    private Vector3 targetScale;

    void Start()
    {
        targetScale = normalScale;
        transform.localScale = normalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData) { targetScale = highlightScale; }
    
    public void OnPointerExit(PointerEventData eventData) { targetScale = normalScale; }
    
    public void OnPointerDown(PointerEventData eventData) { targetScale = pressedScale; }
    
    public void OnPointerUp(PointerEventData eventData) { targetScale = highlightScale; }

    public void OnSelect(BaseEventData eventData) { targetScale = highlightScale; }
    
    public void OnDeselect(BaseEventData eventData) { targetScale = normalScale; }
}