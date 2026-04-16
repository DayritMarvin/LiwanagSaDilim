using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] traps;
    [SerializeField] private Animator anim;

    private int objectsOnPlate = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectsOnPlate++;

        if (objectsOnPlate == 1)
        {
            ActivatePlate();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectsOnPlate--;

        if (objectsOnPlate <= 0)
        {
            objectsOnPlate = 0;
            DeactivatePlate();
        }
    }

    private void ActivatePlate()
    {
        if (anim != null)
            anim.SetBool("Pressed", true);

        foreach (MonoBehaviour trap in traps)
        {
            if (trap != null)
            {
                trap.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void DeactivatePlate()
    {
        if (anim != null)
            anim.SetBool("Pressed", false);

        foreach (MonoBehaviour trap in traps)
        {
            if (trap != null)
            {
                trap.SendMessage("Deactivate", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}