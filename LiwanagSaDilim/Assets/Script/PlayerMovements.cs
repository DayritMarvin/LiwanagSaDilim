using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
   
    private Rigidbody2D Rigidbody2D;
    public float speedMovement ;
    public float jumpForce; 
    private bool isGrounded = false;
    
    void Start (){
            Rigidbody2D = GetComponent <Rigidbody2D> ();

    }
    
    // Player Movements
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal") * speedMovement , Rigidbody2D.velocity.y);

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            jump();
            
        }

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(0.38128f, 0.38128f, 0.38128f);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-0.38128f, 0.38128f, 0.38128f); 
        
    }
    private void jump () 
    {

        Rigidbody2D.velocity = new Vector2 (Rigidbody2D.velocity.x, jumpForce);
        isGrounded = false; 

    }

    private void OnCollisionEnter2D (Collision2D col) 
    {
        if(col.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }
    }

}
