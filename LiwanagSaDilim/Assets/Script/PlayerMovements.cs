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
    private bool facingRight = true;
    private float horizontalInput;

    // 4 animation
    private Animator animation;
    //private bool grounded;

    void Start (){

        // references for rigidbody and animator from object 
            Rigidbody2D = GetComponent <Rigidbody2D> ();
            animation = GetComponent<Animator> ();
           
}
    
    // Player Movements
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        Rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal") * speedMovement , Rigidbody2D.velocity.y);

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            jump();
            
        }

        Direction();

        // set animator parameters
        animation.SetBool("Walk", horizontalInput != 0);
        animation.SetBool("grounded", isGrounded);
    }
    // player jump
    private void jump () 
    {

        Rigidbody2D.velocity = new Vector2 (Rigidbody2D.velocity.x, jumpForce);
        // grounded = false;
        isGrounded = false; 
        animation.SetTrigger("jumpAni");
       
    }

    private void OnCollisionEnter2D (Collision2D col) 
    {
        if(col.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }
    }

    private void Direction()
    {
        if (horizontalInput <0 && facingRight || horizontalInput >0 && !facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(new Vector3(0, 180, 0));
        }
            

    }

}
