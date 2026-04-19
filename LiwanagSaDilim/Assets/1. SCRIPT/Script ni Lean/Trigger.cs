using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTrap : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] traps;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (MonoBehaviour trap in traps)
            {
                if (trap != null)
                {
                    trap.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}