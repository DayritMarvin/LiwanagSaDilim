using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTrap : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private bool activated = false;

    public void Activate()
    {
        if (activated)
            return;

        activated = true;

        // 🎬 Play rope snap animation
        anim.SetTrigger("Activate");
    }
}