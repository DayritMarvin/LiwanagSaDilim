using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyGuide : MonoBehaviour
{
    [Header("Guidepoint here")]
    public List<Transform> waypoints; 

    public float moveSpeed = 5f;        
    public float detectionRadius = 2f;  
    public float rotationSpeed = 10f;   
    public float waitTime = 1f; 

    public Transform player; 

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private float currentWaitTimer = 0f; 

    void Start()
    {
        if (waypoints != null && waypoints.Count > 0 && waypoints[0] != null)
        {
            transform.position = waypoints[0].position;
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0 || player == null || currentWaypointIndex >= waypoints.Count) return; 

        if (!isMoving && currentWaitTimer > 0)
        {
            currentWaitTimer -= Time.deltaTime;
        }

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceToPlayer <= detectionRadius && !isMoving && currentWaitTimer <= 0)
        {
            if (currentWaypointIndex + 1 < waypoints.Count)
            {
                currentWaypointIndex++;
                isMoving = true;
            }
        }

        if (isMoving)
        {
            Transform targetPoint = waypoints[currentWaypointIndex];
            MoveToTarget(targetPoint);
        }
    }

    void MoveToTarget(Transform target)
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        Vector3 direction = target.position - transform.position;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            isMoving = false; 
            currentWaitTimer = waitTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        if (waypoints != null && waypoints.Count > 1)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i+1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
                }
            }
        }
    }
}