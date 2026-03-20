using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSound : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        Vector2 dragVol = new Vector2(rb.velocity.x, 0);
        if(dragVol.magnitude <= 0.05)
        {
            audioSource.Stop();
        }
        else
        {
            if(audioSource.isPlaying) return;
            audioSource.Play();
        }
    }
}
