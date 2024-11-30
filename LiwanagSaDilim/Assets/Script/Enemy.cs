
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public float speed;
    public float distance;
    public bool detection;
    public GameObject player;
   
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
       
    {
        
        if (detection == true)
        {
        transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
           
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
           
             detection = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
       
        detection = false;

    }
}
    