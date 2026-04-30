using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSoundScript : MonoBehaviour
{
    
    public AudioSource audioSource;

    public AudioClip clip;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
