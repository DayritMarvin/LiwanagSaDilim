
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
  
    [SerializeField] private Transform pointA; 
    [SerializeField] private Transform pointB; 
    [SerializeField] private float speed = 2f; 
    [SerializeField] private float threshold = 0.1f; 

    private Transform _targetPoint;
    private bool _facingRight = true; 
    private Animator _animator; 

    private void Start()
    {
        _targetPoint = pointA; 
        _animator = GetComponent<Animator>(); 
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPoint.position, speed * Time.deltaTime);

       
        _animator.SetBool("IsWalking", true);

       
        if (Vector3.Distance(transform.position, _targetPoint.position) <= threshold)
        {
           
            _animator.SetBool("IsWalking", false);

          
            _targetPoint = _targetPoint == pointA ? pointB : pointA;

           
            Flip();
        }
    }

    private void Flip()
    {
        _facingRight = !_facingRight; 

       
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}