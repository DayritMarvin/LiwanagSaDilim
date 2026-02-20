using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System;
using System.Diagnostics;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class PlayerMovements : MonoBehaviour
{
    #region 1. COMPONENTS & REFERENCES
    [Header("--- COMPONENTS ---")]
    private Rigidbody2D rb;
    private Animator anim;
    private Renderer rend;
    private Color originalColor;
    private LevelHandler levelHandler;
    private SoundManager audioManager;
    PlayerVoiceCommand voiceCommand;

    [Header("--- EXTERNAL OBJECTS ---")]
    public GameObject PlayerModel; 
    public GameObject DamageEffect; 
    public GameObject HealthEffect; 
    #endregion

    #region 2. MOVEMENT SETTINGS
    [Header("--- MOVEMENT SETTINGS ---")]
    public float speedMovement = 5f;
    public float jumpForce = 10f;
    
    private float horizontalInput;
    private float mobileInput = 0f; 
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool pushing = false;
    #endregion

    #region 3. HEALTH & LIGHT SETTINGS
    [Header("--- HEALTH & LIGHT ---")]
    public static int lives = 3; 
    public int maxLives = 10;
    
    [Space(5)]
    public Light2D playerLight; 
    public float maxLightRadius = 5f; 
    public float maxLightIntensity = 1.5f;

    public bool damaged = false;
    private bool healed = false;
    private bool isDying = false;
    private float deathTimer = 2.2f;
    #endregion

    #region 4. POWER-UP SYSTEM (RGB)
    
    // --- BLUE (DOUBLE JUMP) ---
    [Header("--- BLUE POWER (DOUBLE JUMP) ---")]
    [HideInInspector] public bool isBlueActive = false;
    private float blueTimer = 0f;
    private bool doubleJumpUsed = false;

    // --- GREEN (DASH) ---
    [Header("--- GREEN POWER (DASH) ---")]
    public GameObject dashButton; 
    public float dashSpeed = 15f;     
    public float dashDuration = 0.2f; 
    public float dashCooldown = 1f;   
    [HideInInspector] public bool isGreenActive = false;
    private float greenTimer = 0f;
    private bool isDashing = false;   
    private bool canDash = true;      

    // --- RED (STRENGTH) ---
    [Header("--- RED POWER (STRENGTH) ---")]
    [HideInInspector] public bool isRedActive = false;
    private float redTimer = 0f;
    // Settings para sa Heavy Box manipulation
    private float normalMass = 1f;    // Bigat kapag malakas si Liyab
    private float heavyMass = 1000f;  // Bigat kapag normal si Liyab

    #endregion

    #region IMPROVEMENT BY NULL
    [Header("--- IMPROVED POWER COMMANDS ---")]
    public PowerUpType currentPower = PowerUpType.None;
    float powerTimer = 0f;

    [Header("--- MOVEMENT CONTROL TYPES ---")]
    public MovementControlType movementControlType = MovementControlType.PC;

    public GameObject[] ButtonUi;
    public GameObject[] AiUi;
    public TMPro.TMP_Dropdown controlDropdown;
    #endregion

    // ---------------------------------------------------------
    // UNITY EVENTS
    // ---------------------------------------------------------
    
    #region UNITY EVENTS
    private void Awake()
    {
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null) 
            audioManager = audioObj.GetComponent<SoundManager>();
    }

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        levelHandler = FindObjectOfType<LevelHandler>();
        voiceCommand = GetComponent<PlayerVoiceCommand>();

        lives = 3;
        UpdateLight();

        //----added by Null------
        currentPower = PowerUpType.None;
        ControlDropdown();
    }

    

    void Update()
    {
        if (Time.timeScale == 0)
        {
            if (audioManager != null) audioManager.StopWalkSound();
            return; 
        }


        //HandlePowerUpTimers();

        
        ProcessInputs();

        //----added by Null------
        ImprovedTimer();
        ImprovedControls();
        //----------------------

        UpdateAnimations();
        HandleEffects();

        if (lives <= 0 && !isDying) isDying = true;
        if (isDying) HandleDeath();
    }

    void FixedUpdate()
    {
        MovementImprovement();
    }

    void LateUpdate()
    {
        if ((horizontalInput < 0 && facingRight) || (horizontalInput > 0 && !facingRight))
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        if (horizontalInput != 0 && isGrounded)
        {
            if(audioManager) audioManager.PlayWalkSound();
        }
        else
        {
            if(audioManager) audioManager.StopWalkSound();
        }
    }

    private void OnDisable()
    {
        if (DamageEffect != null) DamageEffect.SetActive(false);
        if (HealthEffect != null) HealthEffect.SetActive(false);
        if (audioManager != null) audioManager.StopWalkSound();
    }
    #endregion

    // ---------------------------------------------------------
    // CORE LOGIC
    // ---------------------------------------------------------

    #region MOVEMENT LOGIC
    void ProcessInputs()
    {
        //if (isDashing) return;
        //horizontalInput = Input.GetAxisRaw("Horizontal") + mobileInput;
        //horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);

        //if (Input.GetKeyDown(KeyCode.Space)) HandleJumpLogic();
        //if (Input.GetKeyDown(KeyCode.LeftShift) && lives > 0) DashBtn(); // Test key
    }

    public void HandleJumpLogic()
    {
        if(lives <= 0) return;
        if (isGrounded)
        {
            PerformJump();
            doubleJumpUsed = false; 
        }
        else if (isBlueActive && !doubleJumpUsed)
        {
            PerformJump();
            doubleJumpUsed = true; 
        }
    }

    void PerformJump()
    {
        if(audioManager) audioManager.PlayJumpSound();
        rb.velocity = new Vector2(rb.velocity.x, 0); 
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        anim.SetTrigger("jump");
        isGrounded = false;
    }

    #region MOVEMENT IMPROVED BY NULL

    void MovementImprovement()
    {
        if (isDashing) return;
        horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
        rb.velocity = new Vector2(horizontalInput * speedMovement, rb.velocity.y);
    }

    void ImprovedControls()
    {
        //DropdownValueChanged(controlDropdown);

        // if(Application.isMobilePlatform)
        // {
        //     ForMobile();
        // }
        // else
        // {
        //     ForPc();
        // }

        if(Input.GetKeyDown(KeyCode.Alpha1)) movementControlType = MovementControlType.Mobile;
        if(Input.GetKeyDown(KeyCode.Alpha2)) movementControlType = MovementControlType.MobileAi;
        if(Input.GetKeyDown(KeyCode.Alpha3)) movementControlType = MovementControlType.PC;
        if(Input.GetKeyDown(KeyCode.Alpha4)) movementControlType = MovementControlType .PCAi;

        ForMobile();
        ForPc();

        foreach(GameObject btn in ButtonUi)
            { btn.SetActive(movementControlType == MovementControlType.Mobile || movementControlType == MovementControlType.MobileAi);}
        foreach(GameObject btn in AiUi)
        { 
            if(btn == null) return;
            btn.SetActive(movementControlType == MovementControlType.Mobile || movementControlType == MovementControlType.MobileAi);
        }

    }

    void ForMobile()
    {
        if(movementControlType != MovementControlType.Mobile && movementControlType != MovementControlType.MobileAi) return;
        switch(movementControlType)
        {
            case MovementControlType.Mobile:
                
                break;
            case MovementControlType.MobileAi:
                voiceCommand.Active();
                //horizontalInput = Input.acceleration.x + 1;
                break;
        }
        horizontalInput = Input.GetAxisRaw("Horizontal") + mobileInput;

    }

    void ForPc()
    {
        if(movementControlType != MovementControlType.PC && movementControlType != MovementControlType.PCAi) return;
        switch(movementControlType)
        {
            case MovementControlType.PC:
            if (Input.GetKeyDown(KeyCode.R) && lives > 0) ImprovedActivatePower(PowerUpType.Red); 
            if (Input.GetKeyDown(KeyCode.B) && lives > 0) ImprovedActivatePower(PowerUpType.Blue); 
            if (Input.GetKeyDown(KeyCode.G) && lives > 0) ImprovedActivatePower(PowerUpType.Green);
                break;
            case MovementControlType.PCAi:
                voiceCommand.Active();
                break;
        }
        if (Input.GetKeyDown(KeyCode.Space)) HandleJumpLogic();

        horizontalInput = Input.GetAxisRaw("Horizontal");
        
    }

    /// <summary>
    /// FOR TESTING CONTROLS ONLY
    /// </summary>

    void ControlDropdown()
    {
        if (controlDropdown != null)
        {
            PopulateDropdownWithEnum<MovementControlType>(controlDropdown);
            controlDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(controlDropdown); });
            // Set initial value if needed
            controlDropdown.value = (int)movementControlType;
        }
    }

    public static void PopulateDropdownWithEnum<T>(TMPro.TMP_Dropdown dropdown) where T : Enum
    {
        dropdown.ClearOptions();
        List<string> enumNames = new List<string>(Enum.GetNames(typeof(T)));
        dropdown.AddOptions(enumNames);
    }

    void DropdownValueChanged(TMPro.TMP_Dropdown change)
    {
        movementControlType = (MovementControlType)change.value; 
    }

        
    #endregion MOVEMENT IMPROVED BY NULL


    #endregion

    #region ANIMATIONS & EFFECTS
    void UpdateAnimations()
    {
        anim.SetBool("Walk", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded);
        anim.SetBool("push", pushing);
        
        if (pushing) anim.SetTrigger("push");
    }

    void HandleEffects()
    {
        if (damaged) StartCoroutine(InvulnerableRoutine());
        if (healed) StartCoroutine(HealedRoutine());
    }
    #endregion

    #region POWER-UP SYSTEM LOGIC (ALL)
    public void DashBtn()
    {
        if (isGreenActive && canDash && !isDashing && lives > 0) StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isDashing = true; canDash = false;
        float originalGravity = rb.gravityScale; rb.gravityScale = 0; 
        rb.velocity = transform.right * dashSpeed;
        yield return new WaitForSeconds(dashDuration); 
        rb.gravityScale = originalGravity; rb.velocity = Vector2.zero; 
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown); canDash = true;
    }

    #region IMPROVED POWER COMMAND BY NULL
    public void ImprovedActivatePower(PowerUpType curPower)
    {
        currentPower = curPower;
        powerTimer = 5f;
    }

    public void ImprovedPowerButton(int power)
    {
        ImprovedActivatePower((PowerUpType)power);
    }

    //timer for power up, also handles button indicators and other related stuff
    void ImprovedTimer()
    {
        switch (currentPower)
        {
            case PowerUpType.Blue:
                
                break;
            case PowerUpType.Green:
                if(Input.GetKeyDown(KeyCode.LeftShift))
                {
                    DashBtn();
                }
                break;
            case PowerUpType.Red:
                
                break;
            default:
                
                break;
        }
        if(currentPower != PowerUpType.None)
        {
            powerTimer -= 1 * Time.deltaTime;
        }

        if(powerTimer <= 0)
        {
            currentPower = PowerUpType.None;
        }

        isBlueActive = currentPower == PowerUpType.Blue;
        isGreenActive = currentPower == PowerUpType.Green;
        isRedActive = currentPower == PowerUpType.Red;

        if (dashButton != null) dashButton.SetActive(currentPower == PowerUpType.Green);
    }

    #endregion IMPROVED POWER COMMAND BY NULL


    #region Remove Power Up Codes
        
    // --- BLUE POWER (DOUBLE JUMP) ---
    // public void ActivateBluePower()
    // {
    //     DeactivateGreenPower(); 
    //     DeactivateRedPower();

    //     isBlueActive = true;
    //     blueTimer = 5f; 
    //     if (blueIndicator != null) blueIndicator.SetActive(true);
    // }
    // private void DeactivateBluePower()
    // {
    //     isBlueActive = false;
    //     blueTimer = 0f;
    //     if (blueIndicator != null) blueIndicator.SetActive(false);
    // }

    // --- GREEN POWER (DASH) ---
    // public void ActivateGreenPower()
    // {
    //     DeactivateBluePower();
    //     DeactivateRedPower();

    //     isGreenActive = true;
    //     greenTimer = 5f;
    //     if (greenIndicator != null) greenIndicator.SetActive(true);
    //     if (dashButton != null) dashButton.SetActive(true); 
    // }

    // private void DeactivateGreenPower()
    // {
    //     isGreenActive = false;
    //     greenTimer = 0f;
    //     if (greenIndicator != null) greenIndicator.SetActive(false);
    //     if (dashButton != null) dashButton.SetActive(false); 
    // }

    // --- RED POWER (STRENGTH) ---
    // public void ActivateRedPower()
    // {
    //     DeactivateBluePower();
    //     DeactivateGreenPower();

    //     isRedActive = true;
    //     redTimer = 5f;
    //     if (redIndicator != null) redIndicator.SetActive(true);
    // }

    // private void DeactivateRedPower()
    // {
    //     isRedActive = false;
    //     redTimer = 0f;
    //     if (redIndicator != null) redIndicator.SetActive(false);
    // }

    // --- TIMERS ---
    // void HandlePowerUpTimers()
    // {
    //     if (isBlueActive) 
    //     { 
    //         blueTimer -= Time.deltaTime; 
    //         if (blueTimer <= 0) DeactivateBluePower(); 
    //     }

    //     if (isGreenActive) 
    //     { 
    //         greenTimer -= Time.deltaTime; 
    //         if (greenTimer <= 0) DeactivateGreenPower(); 
    //     }

    //     if (isRedActive) 
    //     { 
    //         redTimer -= Time.deltaTime; 
    //         if (redTimer <= 0) DeactivateRedPower(); 
    //     }
    // }
    
    // void ResetPowerUpsUI()
    // {
    //     if(blueIndicator != null) blueIndicator.SetActive(false);
    //     if(greenIndicator != null) greenIndicator.SetActive(false);
    //     if(redIndicator != null) redIndicator.SetActive(false);
    //     if(dashButton != null) dashButton.SetActive(false);
    // }

    #endregion
    #endregion

    #region HEALTH & LIGHT SYSTEM
    public void UpdateLight() { if (playerLight) { float p = (float)lives/maxLives; playerLight.pointLightOuterRadius = maxLightRadius*p; playerLight.intensity = maxLightIntensity*p; } }
    public void AddLife() { if (lives < maxLives) { lives++; healed = true; UpdateLight(); } }
    public void TakeDamage() { if (!damaged) { lives--; damaged = true; UpdateLight(); } }
    private void HandleDeath() { anim.SetTrigger("death"); rb.velocity = new Vector2(0, rb.velocity.y); deathTimer -= Time.deltaTime; if (deathTimer <= 0f) { if(PlayerModel) PlayerModel.SetActive(false); else gameObject.SetActive(false); if (levelHandler) levelHandler.levelFailed(); } }
    #endregion

    #region MOBILE CONTROLS
    // public void MoveLeft() { mobileInput = -1f; }
    // public void MoveRight() { mobileInput = 1f; }
    // public void StopMoving() { mobileInput = 0f; }
    public void JumpBtn() {HandleJumpLogic(); }

    //----Improved by Null------

    public void ButtonMove(int val) {mobileInput = val;}

    ///-------------------------------------

    #endregion

    // ---------------------------------------------------------
    // COLLISIONS (HEAVY BOX LOGIC IS HERE)
    // ---------------------------------------------------------

    #region COLLISIONS
    private void OnCollisionEnter2D(Collision2D col)
    {
        CheckGroundAndPush(col);
        if (col.gameObject.CompareTag("Enemy")) TakeDamage();
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        // Check Physics bawat frame habang nakadikit
        // Ito ang nagma-magic para gumaan ang box kapag Red Active
        if (col.gameObject.CompareTag("HeavyPushable"))
        {
            Rigidbody2D boxRb = col.gameObject.GetComponent<Rigidbody2D>();
            if (boxRb != null)
            {
                if (isRedActive)
                {
                    // Kung may strength, gawing magaan (5)
                    boxRb.mass = 10f; 
                    pushing = true;
                }
                else
                {
                    // Kung wala, gawing sobrang bigat (1000)
                    boxRb.mass = 1000f;
                    // 'Wag mag-animate ng push kasi 'di kaya itulak
                    pushing = false; 
                }
            }
        }
        else if (col.gameObject.CompareTag("Pushable"))
        {
            // Normal box pushing
            pushing = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Pushable") || col.gameObject.CompareTag("HeavyPushable"))
        {
            pushing = false;
            
            // Ibalik sa heavy mass pag umalis na, para sure
            if (col.gameObject.CompareTag("HeavyPushable"))
            {
                Rigidbody2D boxRb = col.gameObject.GetComponent<Rigidbody2D>();
                if(boxRb) boxRb.mass = 1000f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Death")) { lives = 0; UpdateLight(); }
    }

    // Helper para sa Ground Check
    void CheckGroundAndPush(Collision2D col)
    {
        if (col.gameObject.CompareTag("Floor") || 
            col.gameObject.CompareTag("Pushable") || 
            col.gameObject.CompareTag("HeavyPushable")) 
        {
            isGrounded = true;
        }
    }
    #endregion

    #region COROUTINES
    IEnumerator InvulnerableRoutine() 
    { 
        if(DamageEffect) DamageEffect.SetActive(true); 
        Physics2D.IgnoreLayerCollision(7, 8, true); 
        Color c = rend.material.color; c.a = 0.5f; 
        rend.material.color = c; 
        yield return new WaitForSeconds(0.5f); 

        if(DamageEffect) DamageEffect.SetActive(false); 
        yield return new WaitForSeconds(2.5f); 
        Physics2D.IgnoreLayerCollision(7, 8, false); 
        rend.material.color = originalColor; 
        damaged = false; 
    }

    IEnumerator HealedRoutine() { if(HealthEffect) HealthEffect.SetActive(true); yield return new WaitForSeconds(0.5f); if(HealthEffect) HealthEffect.SetActive(false); healed = false; }
    #endregion
}