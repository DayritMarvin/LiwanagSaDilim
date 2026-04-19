using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.UI; 

public class PlayerMovements : MonoBehaviour
{
    #region 1. COMPONENTS & REFERENCES
    [Header("--- COMPONENTS ---")]
    private Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] Transform playerObject;
    [SerializeField]private Renderer rend;
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

    [Header("--- FIREFLY COLORS ---")]
    public Color originalFireflyColor = new Color(1f, 0.9f, 0.2f);
    public Color redPowerColor = Color.red;
    public Color greenPowerColor = Color.green;
    public Color bluePowerColor = Color.cyan; 

    public bool damaged = false;
    private bool healed = false;
    private bool isDying = false;
    private float deathTimer = 2.2f;
    #endregion

    #region 4. POWER-UP SYSTEM (RGB)
    [Header("--- BLUE POWER (DOUBLE JUMP) ---")]
    [HideInInspector] public bool isBlueActive = false;
    private bool doubleJumpUsed = false;

    [Header("--- GREEN POWER (DASH) ---")]
    public GameObject dashButton; 
    public GameObject greenButton;
    public float dashSpeed = 15f;     
    public float dashDuration = 0.2f; 
    public float dashCooldown = 1f; 
    [HideInInspector] public bool isGreenActive = false;
    private bool isDashing = false;   
    private bool canDash = true;      

    [Header("--- RED POWER (STRENGTH) ---")]
    [HideInInspector] public bool isRedActive = false;
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
    public Image blueCooldown;
    public Image greenCooldown;
    public Image redCooldown;
    #endregion

    #region INTERACTION & GRABBING 
    [Header("--- INTERACTION & GRABBING ---")]
    public KeyCode interactKey = KeyCode.E;
    private Rigidbody2D currentBoxToGrab; 
    private FixedJoint2D grabJoint;       
    private bool isGrabbing = false;
    [HideInInspector] public bool interactHeld = false;
    public bool hasPushable = false;
    [SerializeField] float pushDistance = 1f;
    [SerializeField] LayerMask pushableLayer;
    #endregion

    #region IMPROVED MOVEMENT LOGIC
    [Header("Improved Movement Logic")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
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
        originalColor = rend.material.color;
        levelHandler = FindObjectOfType<LevelHandler>();
        voiceCommand = GetComponent<PlayerVoiceCommand>();

        lives = 3;
        UpdateLight();
        UpdateFireflyColors();

        currentPower = PowerUpType.None;
        ControlDropdown();

        grabJoint = gameObject.AddComponent<FixedJoint2D>();
        grabJoint.enabled = false;

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
        HandleGrabbing(); 

        ImprovedTimer();
        ImprovedControls();

        UpdateAnimations();

        if (lives <= 0 && !isDying) isDying = true;


    }

    void FixedUpdate()
    {
        if (isDying) return; 
        MovementImprovement();
        HandlePushing();
    }

    void LateUpdate()
    {
        if (!isGrabbing)
        {
            if ((horizontalInput < 0 && facingRight) || (horizontalInput > 0 && !facingRight))
            {
                facingRight = !facingRight;
                playerObject.Rotate(0f, 180f, 0f);
            }
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
        if(lives <= 0 || isGrabbing) return; 
        
        bool canDoubleJump = isBlueActive && !doubleJumpUsed;
        if (!canJump && !canDoubleJump) return; 

        if (isGrounded)
        {
            PerformJump();
            doubleJumpUsed = false; 
        }
        else if (isBlueActive && !doubleJumpUsed && !isGrounded)
        {
            PerformJump();
            doubleJumpUsed = true; 
        }
    }

    void PerformJump()
    {
        if(audioManager)
        audioManager.PlayJumpSound();
        rb.velocity = new Vector2(rb.velocity.x, 0); 
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        anim.SetTrigger("jump");
        //isGrounded = false;
        StartCoroutine(JumpCooldownRoutine());
    }

    void MovementImprovement()
    {   
        //eto na yung sa ground gamit ang raycast para mas accurate yung pagdetect ng ground
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, Color.red);



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
    #endregion

    #region ANIMATIONS
    void UpdateAnimations()
    {
        anim.SetBool("Walk", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded);
        anim.SetBool("push", pushing || isGrabbing); 
        
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
        rb.velocity = playerObject.right * dashSpeed;
        
        yield return new WaitForSeconds(dashDuration); 
        
        rb.gravityScale = originalGravity; 
        rb.velocity = Vector2.zero; 
        isDashing = false;
        rend.material.color = originalColor;

        if (!damaged) Physics2D.IgnoreLayerCollision(7, 8, false);

        yield return new WaitForSeconds(dashCooldown); 
        canDash = true;
    }

    public void ImprovedActivatePower(PowerUpType curPower)
    {
        if (currentPower != PowerUpType.None) return; 

        currentPower = curPower;
        powerTimer = 5f;
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
            if (currentPower != PowerUpType.None)
            {
                currentPower = PowerUpType.None;
                UpdateFireflyColors();
            }
        }

        isBlueActive = currentPower == PowerUpType.Blue;
        isGreenActive = currentPower == PowerUpType.Green;
        isRedActive = currentPower == PowerUpType.Red;

        // --- ITO ANG NAGPAPALIT SA G AT D BUTTONS ---
        if (dashButton != null) dashButton.SetActive(currentPower == PowerUpType.Green);
        if (greenButton != null) greenButton.SetActive(currentPower != PowerUpType.Green);
    }

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
                Light2D light = followingFireflies[i].GetComponent<Light2D>();
                if (light != null) light.color = targetColor;
                SpriteRenderer sr = followingFireflies[i].GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = targetColor;
            }
        }
    }
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

    #region MOBILE CONTROLS & INTERACTION
    public void JumpBtn() {HandleJumpLogic(); }
    public void ButtonMove(int val) {mobileInput = val;}

    // --- BAGO: Updated HandleGrabbing para sa Heavy Boxes ---

    //Instead of using collision, use raycast
    void HandlePushing()
    {
        hasPushable = Physics2D.Raycast(transform.position, playerObject.right, pushDistance, pushableLayer);
        if(hasPushable)
        {
            if(isGrabbing)return;
            RaycastHit2D hit = Physics2D.Raycast(playerObject.position, playerObject.right, pushDistance, pushableLayer);
            Rigidbody2D boxRb = hit.transform.GetComponent<Rigidbody2D>();
            if (boxRb != null)
            {
                if (hit.transform.CompareTag("HeavyPushable"))
                {
                    boxRb.mass = isRedActive ? 10f : 1000f;
                    pushing = true;
                }
                else
                {
                    pushing = true;
                }
                currentBoxToGrab = boxRb;
            }
        }
        else
        {
            currentBoxToGrab = null;
            pushing = false;
        }
        Debug.DrawRay(playerObject.position, playerObject.right * pushDistance, Color.blue);
    }

    void HandleGrabbing()
    {
        if(currentBoxToGrab == null) return;
        bool tryingToGrab = Input.GetKey(interactKey) || interactHeld;

        if (tryingToGrab)
        {
            if (!isGrabbing)
            {
                // if (currentBoxToGrab.gameObject.CompareTag("HeavyPushable"))
                // {
                //     rb.constraints = isRedActive ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
                // }
                // else
                // {
                //     rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                // }

                grabJoint.connectedBody = currentBoxToGrab;
                grabJoint.enabled = true;
                isGrabbing = true;
            }
            else
            {
                if (currentBoxToGrab.gameObject.CompareTag("HeavyPushable"))
                {
                    rb.constraints = isRedActive ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }
        else
        {
            if (isGrabbing)
            {
                //Tinanggal ko muna kasi di naman kailangan
                // if (grabJoint.connectedBody != null && grabJoint.connectedBody.gameObject.CompareTag("HeavyPushable"))
                // {
                //     grabJoint.connectedBody.mass = 1000f;
                // }
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                grabJoint.enabled = false;
                grabJoint.connectedBody = null;
                isGrabbing = false;
                
                // --- BUG FIX: Kalimutan agad ang box pagkabitaw ng E button ---
                currentBoxToGrab = null; 
            }
        }

        // Kapag naubos ang Red Power habang humahatak
        //Tinanggal ko muna kasi hindi naman na kailangan
        // if (isGrabbing && grabJoint.connectedBody != null && grabJoint.connectedBody.gameObject.CompareTag("HeavyPushable"))
        // {
        //     if (isRedActive)
        //     {
        //         grabJoint.connectedBody.mass = 10f; 
        //     }
        //     else
        //     {
        //         grabJoint.connectedBody.mass = 1000f;
        //         grabJoint.enabled = false;
        //         grabJoint.connectedBody = null;
        //         isGrabbing = false;
                
        //         // --- BUG FIX: Kalimutan din ang box kapag na-force bitaw ---
        //         currentBoxToGrab = null;
        //     }
        // }
    }

    public void InteractHoldDown() { interactHeld = true; }
    public void InteractHoldUp() { interactHeld = false; }
    #endregion

    // ---------------------------------------------------------
    // COLLISIONS
    // ---------------------------------------------------------

    #region COLLISIONS
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy")) TakeDamage();
        if (col.gameObject.CompareTag("Death")) { lives = 0; UpdateLight(); }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        //Nilipat na sa HandlePushing na method para mas malinis

        // if (col.gameObject.CompareTag("HeavyPushable"))
        // {
        //     Rigidbody2D boxRb = col.gameObject.GetComponent<Rigidbody2D>();
        //     if (boxRb != null)
        //     {
        //         if (!isGrabbing) boxRb.mass = isRedActive ? 10f : 1000f; 
        //         pushing = true;
        //         if (!isGrabbing) currentBoxToGrab = boxRb;
        //     }
        // }
        // else if (col.gameObject.CompareTag("Pushable"))
        // {
        //     pushing = true;
        //     if (!isGrabbing) currentBoxToGrab = col.gameObject.GetComponent<Rigidbody2D>();
        // }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        // --- BAGO: Katulad ng E button, kalimutan ang pagtalon kapag umalis sa lapag ---
        //tinanggal na para mas maganda ang pagtalon kahit na nasa gilid lang ng platform, at para hindi ma-reset yung jump cooldown kapag umalis sa lapag habang tumatalon
        // if (col.gameObject.CompareTag("Floor") || col.gameObject.CompareTag("Pushable") || col.gameObject.CompareTag("HeavyPushable"))
        // {
        //     isGrounded = false;
        // }

        // --- DATING CODE MO PARA SA MGA BOX (Walang binago) ---
        // if (col.gameObject.CompareTag("Pushable") || col.gameObject.CompareTag("HeavyPushable"))
        // {
        //     pushing = false;
            
        //     if (col.gameObject.CompareTag("HeavyPushable"))
        //     {
        //         Rigidbody2D boxRb = col.gameObject.GetComponent<Rigidbody2D>();
        //         if (boxRb != null && (!isGrabbing || grabJoint.connectedBody != boxRb))
        //         {
        //             boxRb.mass = 1000f;
        //         }
        //     }

        //     if (currentBoxToGrab != null && col.gameObject == currentBoxToGrab.gameObject)
        //     {
        //         currentBoxToGrab = null;
        //     }
        // }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy")) TakeDamage();
        if (col.gameObject.CompareTag("Death")) { lives = 0; UpdateLight(); }
    }

    //hindi na nagamit kasi raycast na pang detect ng ground
    void CheckGroundAndPush(Collision2D col)
    {
        if (col.gameObject.CompareTag("Floor") || col.gameObject.CompareTag("Pushable") || col.gameObject.CompareTag("HeavyPushable")) 
        {
            //isGrounded = true;
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
            c.a = 0.2f; rend.material.color = c; yield return new WaitForSeconds(flickerInterval);
            c.a = 1f; rend.material.color = c; yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval * 2;
            if (timer >= 0.5f && DamageEffect != null && DamageEffect.activeSelf) DamageEffect.SetActive(false);
        }

        Physics2D.IgnoreLayerCollision(7, 8, false); 
        rend.material.color = originalColor; 
        damaged = false; 
    }

    IEnumerator HealedRoutine() { if(HealthEffect) HealthEffect.SetActive(true); yield return new WaitForSeconds(0.5f); if(HealthEffect) HealthEffect.SetActive(false); healed = false; }
    #endregion
}