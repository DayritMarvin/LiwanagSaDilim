using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private bool triggered = false;

    public void Activate()
    {
        if (triggered)
            return;

        triggered = true;
        anim.SetTrigger("Activate");
    }
}