using UnityEngine;

public class FireflyHover : MonoBehaviour
{
    [Header("--- FOLLOW SETTINGS ---")]
    public Transform playerTarget; // Dito natin ilalagay si Liyab
    public float followSpeed = 3f; // Gaano kabilis sumunod ang firefly

    [Header("--- HOVER SETTINGS ---")]
    public float hoverSpeed = 2f;      
    public float hoverAmount = 0.5f;   

    private Vector3 randomOffset; 
    private float timeOffset;

    void Start()
    {
        // Bigyan ang bawat firefly ng sarili nilang random pwesto sa paligid ni Liyab
        // Para kapag nag-spawn sila, kalat-kalat sila.
        randomOffset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0.5f, 2f), 0f);
        
        // Random timing para hindi sabay-sabay ang pag-float nila
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Kung walang susundan, wag gumalaw
        if (playerTarget == null) return;

        // 1. Alamin ang target position (Pwesto ni Liyab + Random na pwesto ng firefly)
        Vector3 targetPosition = playerTarget.position + randomOffset;

        // 2. Idagdag ang floating/hovering effect
        float hoverX = Mathf.Sin(Time.time * hoverSpeed + timeOffset) * hoverAmount;
        float hoverY = Mathf.Cos(Time.time * (hoverSpeed * 0.8f) + timeOffset) * hoverAmount;
        
        targetPosition += new Vector3(hoverX, hoverY, 0f);

        // 3. Smooth na lumipad papunta sa target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}