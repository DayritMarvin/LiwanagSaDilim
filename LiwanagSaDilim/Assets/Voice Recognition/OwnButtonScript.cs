using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class OwnButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnityEvent pressedEvent;
    public UnityEvent unPressedEvent;

    Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressedEvent?.Invoke();
        image.color = Color.green;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unPressedEvent?.Invoke();
        image.color = Color.red;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        unPressedEvent?.Invoke();
        image.color = Color.red;
    }

    
}
