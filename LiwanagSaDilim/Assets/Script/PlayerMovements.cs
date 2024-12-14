using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{

    private Rigidbody2D Rigidbody2D;
    public float speedMovement;
    public float jumpForce;
    private bool isGrounded = false;
    private bool facingRight = true;
    private float horizontalInput;
    public bool damaged = false;
    public static int lives = 3;
    private bool pushing = false;
    public GameObject Player;
    public GameObject GameOver;
    public GameObject effectCanvas;
   
    private bool isDying = false;
    private float deathTimer = 2.2f;
   
    private Animator animation;
  


    //invulnerable
    Renderer rend;
    Color c;
        void Start() {
       
        // references for rigidbody and animator from object 
        Rigidbody2D = GetComponent<Rigidbody2D>();
        animation = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        c = rend.material.color;

        }


        void Update()
        {
        horizontalInput = Input.GetAxis("Horizontal");
        Rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal") * speedMovement, Rigidbody2D.velocity.y);

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            jump();

        }
        // set animator parameters
        animation.SetBool("Walk", horizontalInput != 0);
        animation.SetBool("grounded", isGrounded);
        animation.SetBool("push",pushing);
        Direction();

        if (damaged == true)
        {
            
            StartCoroutine("Invulnerable");
           
        }
        
        if (pushing == true)
        {
            animation.SetTrigger("push");
           
        }
        if (lives <= 0 && !isDying)
        {
            isDying = true; // Start the countdown
           
        }

        // Countdown logic
        if (isDying)
        {
            animation.SetTrigger("death");
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            deathTimer -= Time.deltaTime; // Decrease the timer

            if (deathTimer <= 0f)
            {
                Player.SetActive(false); // Deactivate the player GameObject
                GameOver.SetActive(true);
                
            }
        }

  


        }
       

    

    


// player jump
private void jump()
    {

        Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, jumpForce);
        // grounded = false;
        animation.SetTrigger("jump");
        isGrounded = false;


    }
    IEnumerator Invulnerable()
    {
        effectCanvas.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        effectCanvas.SetActive(false);
        Physics2D.IgnoreLayerCollision(7, 8, true);
        c.a = 0.5f;
        rend.material.color = c;
        yield return new WaitForSeconds(3f);
        Physics2D.IgnoreLayerCollision(7, 8, false);
        c.a = 1f;
        rend.material.color = c;
        damaged = false;
       
    }
  

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Floor" || col.gameObject.tag =="Pushable")
        {
            isGrounded = true;
        }
       
       if (col.gameObject.tag =="Pushable" )
        {
            pushing = true;
        }
        else
        {
            pushing = false;
           
        }
        if (col.gameObject.tag == "Enemy")
        {
            damaged = true;
            lives -= 1;
           
        }
        
        
       
    }

    private void OnTriggerEnter2D (Collider2D col)
    {
      

        if (col.gameObject.tag == "Death")
        {
           
            lives *= 0;
          
        }
    }



    private void Direction()
    {
        if (horizontalInput < 0 && facingRight || horizontalInput > 0 && !facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }

   

    }
   
   


