using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class PlayerMovements : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speedMovement;
    public float jumpForce;
    private LevelHandler levelHandler;
    
    // FLAGS
    private bool isGrounded = false;
    private bool facingRight = true;
    private float horizontalInput;
    public bool damaged = false;
    private bool pushing = false;
    private bool healed = false;
    private bool isDying = false;

    // HEALTH & LIGHT SETTINGS
    public static int lives = 3; 
    public int maxLives = 10;
    public Light2D playerLight;
    public float maxLightRadius = 5f;
    public float maxLightIntensity = 1.5f;

    // UI & EFFECTS
    public GameObject Player;
    public GameObject GameOver;
    public GameObject effectCanvas;
    public GameObject effectCanvas2;
    
    // DEATH TIMER
    private float deathTimer = 2.2f;
    
    // ANIMATOR & AUDIO
    private Animator anim;
    private Renderer rend;
    private Color c;
    SoundManager audioManager;

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
        c = rend.material.color;

        lives = 3; 

        UpdateLight();

        levelHandler = FindObjectOfType<LevelHandler>();
    }

    void Update()
    {
        // MOVEMENT
        horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speedMovement, rb.velocity.y);

        // JUMP
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && lives > 0)
        {
            if(audioManager) audioManager.PlayJumpSound();
            Jump();
        }

        // ANIMATION SETTINGS
        anim.SetBool("Walk", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded);
        anim.SetBool("push", pushing);
        
        CheckDirection();

        // AUDIO WALK
        if (horizontalInput != 0 && isGrounded)
        {
            if(audioManager) audioManager.PlayWalkSound();
        }
        else
        {
            if(audioManager) audioManager.StopWalkSound();
        }

        // --- FIX: STOP SOUND KAPAG STOP TIME ---
        if (Time.timeScale == 0)
        {
            if (audioManager != null)
            {
                audioManager.StopWalkSound();
            }
            return; // Itigil na ang pagbasa ng iba pang input
        }
        
        horizontalInput = Input.GetAxis("Horizontal");

        // EFFECTS LOGIC
        if (damaged) StartCoroutine(Invulnerable());
        if (healed) StartCoroutine(HealedEffect());
        if (pushing) anim.SetTrigger("push");

        // DEATH LOGIC
        if (lives <= 0 && !isDying)
        {
            isDying = true;
        }

        if (isDying)
        {
            HandleDeath();
        }
    }

    // --- CUSTOM FUNCTIONS ---

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        anim.SetTrigger("jump");
        isGrounded = false;
    }

    private void CheckDirection()
    {
        if ((horizontalInput < 0 && facingRight) || (horizontalInput > 0 && !facingRight))
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    // FIX #3: LIGHT LOGIC
    public void UpdateLight()
    {
        if (playerLight == null) return;

        // Calculate percentage (Health / MaxHealth)
        float lightPercent = (float)lives / maxLives;

        // Update Light 2D properties
        playerLight.pointLightOuterRadius = maxLightRadius * lightPercent;
        playerLight.intensity = maxLightIntensity * lightPercent;
    }

    public void AddLife()
    {
        if (lives < maxLives)
        {
            lives++;
            healed = true;
            UpdateLight();
        }
    }

    public void TakeDamage()
    {
        if (!damaged)
        {
            lives--;
            damaged = true;
            UpdateLight();
        }
    }

    private void HandleDeath()
    {
        anim.SetTrigger("death");
        rb.velocity = new Vector2(0, rb.velocity.y);
        deathTimer -= Time.deltaTime;

        if (deathTimer <= 0f)
        {
            Player.SetActive(false);
            if (levelHandler != null)
            {
                levelHandler.levelFailed(); 
            }
        }
    }

    // --- COROUTINES ---

    IEnumerator Invulnerable()
    {
        if(effectCanvas) effectCanvas.SetActive(true);
        
        // Visual flashing
        Physics2D.IgnoreLayerCollision(7, 8, true); // Siguraduhing tama ang Layer Numbers mo dito!
        c.a = 0.5f;
        rend.material.color = c;
        
        yield return new WaitForSeconds(0.5f);
        if(effectCanvas) effectCanvas.SetActive(false);

        yield return new WaitForSeconds(2.5f); // Total wait
        
        Physics2D.IgnoreLayerCollision(7, 8, false);
        c.a = 1f;
        rend.material.color = c;
        damaged = false;
    }

    IEnumerator HealedEffect()
    {
        if(effectCanvas2) effectCanvas2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        if(effectCanvas2) effectCanvas2.SetActive(false);
        healed = false;
    }

    // --- COLLISIONS ---

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Floor") || col.gameObject.CompareTag("Pushable"))
        {
            isGrounded = true;
        }
        
        if (col.gameObject.CompareTag("Pushable"))
        {
            pushing = true;
        }
        else
        {
            pushing = false;
        }

        if (col.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Pushable"))
        {
            pushing = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Death"))
        {
            lives = 0;
            UpdateLight();
        }
    }

    private void OnDisable()
    {
        if (effectCanvas != null)
        {
            effectCanvas.SetActive(false);
        }

        if (effectCanvas2 != null)
        {
            effectCanvas2.SetActive(false);
        }

        if (audioManager != null)
        {
            audioManager.StopWalkSound();
        }
    }
}