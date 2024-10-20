using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
   
   private Rigidbody2D Rigidbody2D;
   public float speedMovement ;
   public float jumpForce; 


    void Start (){
            Rigidbody2D = GetComponent <Rigidbody2D> ();

    }
    
    // Player Movements
    void Update()
    {
        Rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal") * speedMovement , Rigidbody2D.velocity.y);

        if (Input.GetKey(KeyCode.Space))
        {
            jump();
        }
        
    }
    private void jump () 
    {
        Rigidbody2D.velocity = new Vector2 (Rigidbody2D.velocity.x, jumpForce);


    }
}
