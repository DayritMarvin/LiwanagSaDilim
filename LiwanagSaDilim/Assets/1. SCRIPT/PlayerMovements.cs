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
    
    [Header("--- JUMP COOLDOWN ---")]
    public float jumpCooldown = 0.5f;
    private bool canJump = true; 

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

    [Header("--- FOLLOWING FIREFLIES ---")]
    public GameObject[] followingFireflies; 

    // --- BAGO: Mga Kulay para sa Fireflies ---
    [Header("--- FIREFLY COLORS ---")]
    public Color originalFireflyColor = new Color(1f, 0.9f, 0.2f); // Default na medyo yellow
    public Color redPowerColor = Color.red;
    public Color greenPowerColor = Color.green;
    public Color bluePowerColor = Color.cyan; // Cyan para mas maganda ang glow kaysa dark blue

    public bool damaged = false;
    private bool healed = false;
    private bool isDying = false;
    private float deathTimer = 2.2f;
    #endregion

    #region 4. POWER-UP SYSTEM (RGB)
    
    [Header("--- BLUE POWER (DOUBLE JUMP) ---")]
    [HideInInspector] public bool isBlueActive = false;
    private float blueTimer = 0f;
    private bool doubleJumpUsed = false;

    [Header("--- GREEN POWER (DASH) ---")]
    public GameObject dashButton; 
    public float dashSpeed = 15f;     
    public float dashDuration = 0.2f; 
    public float dashCooldown = 1f;   
    [HideInInspector] public bool isGreenActive = false;
    private float greenTimer = 0f;
    private bool isDashing = false;   
    private bool canDash = true;      

    [Header("--- RED POWER (STRENGTH) ---")]
    [HideInInspector] public bool isRedActive = false;
    private float redTimer = 0f;
    private float normalMass = 1f;
    private float heavyMass = 1000f;

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

    [Header("--- POWER UP COOLDOWN UI ---")]
    public UnityEngine.UI.Image blueCooldown;
    public UnityEngine.UI.Image greenCooldown;
    public UnityEngine.UI.Image redCooldown;
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

        currentPower = PowerUpType.None;
        ControlDropdown();
        
        // --- BAGO: I-set ang original na kulay pagka-start ng laro ---
        UpdateFireflyColors();

        Physics2D.IgnoreLayerCollision(7, 8, false);
        damaged = false;
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            if (audioManager != null) audioManager.StopWalkSound();
            return; 
        }

        if (isDying)
        {
            HandleDeath();
            return; 
        }

        ProcessInputs();

        ImprovedTimer();
        ImprovedControls();

        UpdateAnimations();

        if (lives <= 0 && !isDying) isDying = true;
    }

    void FixedUpdate()
    {
        if (isDying) return; 
        
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
    void ProcessInputs() { }

    public void HandleJumpLogic()
    {
        if(lives <= 0) return;
        bool canDoubleJump = isBlueActive && !doubleJumpUsed;
        if (!canJump && !canDoubleJump) 
        {
            return; 
        }

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

        StartCoroutine(JumpCooldownRoutine());
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
            btn.SetActive(movementControlType == MovementControlType.Mobile);
        }
    }

    void ForMobile()
    {
        if(movementControlType != MovementControlType.Mobile && movementControlType != MovementControlType.MobileAi) return;
        switch(movementControlType)
        {
            case MovementControlType.Mobile:
                horizontalInput = Input.GetAxisRaw("Horizontal") + mobileInput;
                break;
            case MovementControlType.MobileAi:
                voiceCommand.Active();
                horizontalInput = Input.acceleration.x + 1;
                break;
        }
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

    void ControlDropdown()
    {
        if (controlDropdown != null)
        {
            PopulateDropdownWithEnum<MovementControlType>(controlDropdown);
            controlDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(controlDropdown); });
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
    #endregion

    #region POWER-UP SYSTEM LOGIC (ALL)
    public void DashBtn()
    {
        if (isGreenActive && canDash && !isDashing && lives > 0) StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isDashing = true; 
        canDash = false;

        Physics2D.IgnoreLayerCollision(7, 8, true);

        Color dashColor = rend.material.color;
        dashColor.a = 0.5f;
        rend.material.color = dashColor;

        float originalGravity = rb.gravityScale; 
        rb.gravityScale = 0; 
        rb.velocity = transform.right * dashSpeed;
        
        yield return new WaitForSeconds(dashDuration); 
        
        rb.gravityScale = originalGravity; 
        rb.velocity = Vector2.zero; 
        isDashing = false;

        rend.material.color = originalColor;

        if (!damaged)
        {
            Physics2D.IgnoreLayerCollision(7, 8, false);
        }

        yield return new WaitForSeconds(dashCooldown); 
        canDash = true;
    }

    #region IMPROVED POWER COMMAND BY NULL
    public void ImprovedActivatePower(PowerUpType curPower)
    {
        currentPower = curPower;
        powerTimer = 5f;
        
        // --- BAGO: I-update ang kulay ng fireflies kapag nag-activate ng power ---
        UpdateFireflyColors(); 
    }

    public void ImprovedPowerButton(int power)
    {
        ImprovedActivatePower((PowerUpType)power);
    }

    void ImprovedTimer()
    {
        switch (currentPower)
        {
            case PowerUpType.Blue: break;
            case PowerUpType.Green:
                if(Input.GetKeyDown(KeyCode.LeftShift)) { DashBtn(); }
                break;
            case PowerUpType.Red: break;
            default: break;
        }
        
        if (blueCooldown != null) blueCooldown.fillAmount = 0f;
        if (greenCooldown != null) greenCooldown.fillAmount = 0f;
        if (redCooldown != null) redCooldown.fillAmount = 0f;

        if(currentPower != PowerUpType.None)
        {
            powerTimer -= 1 * Time.deltaTime;
            float fillValue = powerTimer / 5f; 

            if (currentPower == PowerUpType.Blue && blueCooldown != null) 
                blueCooldown.fillAmount = fillValue;
            else if (currentPower == PowerUpType.Green && greenCooldown != null) 
                greenCooldown.fillAmount = fillValue;
            else if (currentPower == PowerUpType.Red && redCooldown != null) 
                redCooldown.fillAmount = fillValue;
        }

        if(powerTimer <= 0)
        {
            // --- BAGO: Ibalik sa original na kulay kapag naubos na ang timer ---
            if (currentPower != PowerUpType.None)
            {
                currentPower = PowerUpType.None;
                UpdateFireflyColors();
            }
        }

        isBlueActive = currentPower == PowerUpType.Blue;
        isGreenActive = currentPower == PowerUpType.Green;
        isRedActive = currentPower == PowerUpType.Red;

        if (dashButton != null) dashButton.SetActive(currentPower == PowerUpType.Green);
    }

    // --- BAGO: Function para magpalit ng kulay ang mga Fireflies ---
    void UpdateFireflyColors()
    {
        if (followingFireflies == null) return;

        Color targetColor = originalFireflyColor;

        if (currentPower == PowerUpType.Red) targetColor = redPowerColor;
        else if (currentPower == PowerUpType.Green) targetColor = greenPowerColor;
        else if (currentPower == PowerUpType.Blue) targetColor = bluePowerColor;

        for (int i = 0; i < followingFireflies.Length; i++)
        {
            if (followingFireflies[i] != null)
            {
                // Palitan ang kulay ng ilaw
                Light2D light = followingFireflies[i].GetComponent<Light2D>();
                if (light != null) light.color = targetColor;

                // Palitan din ang kulay ng Sprite (kung nilagyan mo ng tuldok/sprite yung firefly)
                SpriteRenderer sr = followingFireflies[i].GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = targetColor;
            }
        }
    }
    #endregion IMPROVED POWER COMMAND BY NULL

    #endregion

    #region HEALTH & LIGHT SYSTEM
    public void UpdateLight() 
    { 
        if (playerLight) 
        { 
            float p = (float)lives/maxLives; 
            playerLight.pointLightOuterRadius = maxLightRadius*p; 
            playerLight.intensity = maxLightIntensity*p; 
        } 
        
        if (followingFireflies != null)
        {
            for (int i = 0; i < followingFireflies.Length; i++)
            {
                if (followingFireflies[i] != null)
                {
                    followingFireflies[i].SetActive(i < lives);
                }
            }
        }
    }
    
    public void AddLife() 
    { 
        if (lives < maxLives) 
        { 
            lives++; 
            healed = true; 
            UpdateLight(); 
            StartCoroutine(HealedRoutine()); 
        } 
    }
    
    public void TakeDamage() 
    { 
        if (!damaged) 
        { 
            lives--; 
            damaged = true; 
            UpdateLight(); 
            StartCoroutine(InvulnerableRoutine()); 
        } 
    }
    
    private void HandleDeath() 
    { 
        anim.SetTrigger("death"); 
        horizontalInput = 0f;
        rb.velocity = new Vector2(0, rb.velocity.y); 
        deathTimer -= Time.deltaTime; 
        if (deathTimer <= 0f) 
        { 
            if(PlayerModel) PlayerModel.SetActive(false); 
            else gameObject.SetActive(false); 
            if (levelHandler) levelHandler.levelFailed(); 
        } 
    }
    #endregion

    #region MOBILE CONTROLS
    public void JumpBtn() {HandleJumpLogic(); }
    public void ButtonMove(int val) {mobileInput = val;}
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
        if (col.gameObject.CompareTag("HeavyPushable"))
        {
            Rigidbody2D boxRb = col.gameObject.GetComponent<Rigidbody2D>();
            if (boxRb != null)
            {
                if (isRedActive)
                {
                    boxRb.mass = 10f; 
                    pushing = true;
                }
                else
                {
                    boxRb.mass = 1000f;
                    pushing = false; 
                }
            }
        }
        else if (col.gameObject.CompareTag("Pushable"))
        {
            pushing = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Pushable") || col.gameObject.CompareTag("HeavyPushable"))
        {
            pushing = false;
            
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
    IEnumerator JumpCooldownRoutine()
    {
        canJump = false; 
        yield return new WaitForSeconds(jumpCooldown); 
        canJump = true;  
    }

    IEnumerator InvulnerableRoutine() 
    { 
        if(DamageEffect) DamageEffect.SetActive(true); 
        Physics2D.IgnoreLayerCollision(7, 8, true); 
        
        float invulnerableDuration = 3f; 
        float flickerInterval = 0.15f; 
        float timer = 0f;

        Color c = rend.material.color;

        while (timer < invulnerableDuration)
        {
            c.a = 0.2f; 
            rend.material.color = c;
            yield return new WaitForSeconds(flickerInterval);

            c.a = 1f; 
            rend.material.color = c;
            yield return new WaitForSeconds(flickerInterval);

            timer += flickerInterval * 2;

            if (timer >= 0.5f && DamageEffect != null && DamageEffect.activeSelf)
            {
                DamageEffect.SetActive(false);
            }
        }

        Physics2D.IgnoreLayerCollision(7, 8, false); 
        rend.material.color = originalColor; 
        damaged = false; 
    }

    IEnumerator HealedRoutine() { if(HealthEffect) HealthEffect.SetActive(true); yield return new WaitForSeconds(0.5f); if(HealthEffect) HealthEffect.SetActive(false); healed = false; }
    #endregion
}