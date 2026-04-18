using UnityEngine;
using UnityEngine.EventSystems; // Importante ito para maka-detect ng mouse hovers at clicks

public class ButtonScaleAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    [Header("Scale Settings")]
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);     // Normal na laki
    public Vector3 highlightScale = new Vector3(1.1f, 1.1f, 1.1f); // Lalaki ng 10% kapag tinapatan
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);   // Lulubog nang konti kapag pinindot

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
        // Lerp para napaka-smooth ng paglaki at pagliit
        // Gumamit tayo ng unscaledDeltaTime para gumana kahit naka-pause ang laro
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animSpeed);
    }

    // --- MGA AUTOMATIC TRIGGERS ---

    // Kapag tinapatan ng mouse
    public void OnPointerEnter(PointerEventData eventData) { targetScale = highlightScale; }
    
    // Kapag inalis ang mouse
    public void OnPointerExit(PointerEventData eventData) { targetScale = normalScale; }
    
    // Kapag nakadiin ang click
    public void OnPointerDown(PointerEventData eventData) { targetScale = pressedScale; }
    
    // Kapag binitawan ang click
    public void OnPointerUp(PointerEventData eventData) { targetScale = highlightScale; }

    // Kapag na-select gamit ang keyboard o controller
    public void OnSelect(BaseEventData eventData) { targetScale = highlightScale; }
    
    // Kapag na-deselect
    public void OnDeselect(BaseEventData eventData) { targetScale = normalScale; }
}