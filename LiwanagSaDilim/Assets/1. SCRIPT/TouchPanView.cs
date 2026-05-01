using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPanView : MonoBehaviour
{
    [Header("Settings ng Pagsilip")]
    public float panSpeed = 0.015f; 
    public float maxPanDistance = 3f; 
    public float returnSpeed = 4f; 
    public bool invertPan = true;

    private Vector3 targetLocalPosition;
    private bool isPanning = false;

    void Start()
    {
        targetLocalPosition = Vector3.zero;
    }

    void Update()
    {
        bool pressingBegan = false;
        bool pressingHeld = false;
        bool pressingReleased = false;
        Vector2 deltaMove = Vector2.zero;
        bool overUI = false;

        // 1. KUNG SA CELLPHONE (Touch)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (EventSystem.current != null)
                overUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
            
            if (touch.phase == TouchPhase.Began) pressingBegan = true;
            if (touch.phase == TouchPhase.Moved) { pressingHeld = true; deltaMove = touch.deltaPosition; }
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) pressingReleased = true;
        }
        // 2. KUNG SA PC (Mouse Click & Drag sa Unity Editor)
        else
        {
            if (EventSystem.current != null)
                overUI = EventSystem.current.IsPointerOverGameObject();

            if (Input.GetMouseButtonDown(0)) pressingBegan = true;
            if (Input.GetMouseButton(0)) 
            { 
                pressingHeld = true; 
                // Pinalaki natin ang numero ng mouse para pumantay sa sensitivity ng touch screen
                deltaMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 40f; 
            }
            if (Input.GetMouseButtonUp(0)) pressingReleased = true;
        }

        // --- ANG LOGIC NG PAGGALAW ---
        if (pressingBegan)
        {
            // Siguraduhing hindi pinindot ang UI (tulad ng D-Pad o Jump Button)
            if (!overUI) isPanning = true;
        }
        else if (pressingHeld && isPanning)
        {
            float panX = deltaMove.x * panSpeed;
            float panY = deltaMove.y * panSpeed;

            if (invertPan)
            {
                targetLocalPosition += new Vector3(-panX, -panY, 0); 
            }
            else
            {
                targetLocalPosition += new Vector3(panX, panY, 0); 
            }

            // I-limit ang galaw
            targetLocalPosition.x = Mathf.Clamp(targetLocalPosition.x, -maxPanDistance, maxPanDistance);
            targetLocalPosition.y = Mathf.Clamp(targetLocalPosition.y, -maxPanDistance, maxPanDistance);
        }
        else if (pressingReleased)
        {
            isPanning = false;
        }

        // Pagkabitaw, babalik sa gitna
        if (!isPanning)
        {
            targetLocalPosition = Vector3.Lerp(targetLocalPosition, Vector3.zero, Time.deltaTime * returnSpeed);
        }

        // I-apply ang movement sa camera target
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPosition, Time.deltaTime * 10f);
    }
}