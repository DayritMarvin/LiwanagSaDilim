using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour

{
    [SerializeField] AudioSource musicSource;
     [SerializeField] AudioSource SFXSource;

     [Header("--------- Audio Clip ---------")]

    public AudioClip background;
    public AudioClip jump;
    public AudioClip walk;
    public AudioClip firefly;
    public AudioClip push;



    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    
    public void PlaySFX(AudioClip clip)
    {

        SFXSource.PlayOneShot(clip);
        
    }

    

    public void PlayWalkSound()
    {
        if (SFXSource.clip != walk || !SFXSource.isPlaying)
        {
            SFXSource.clip = walk;
            SFXSource.loop = true; // Enable looping
            SFXSource.Play();
        }
         

    }

    public void StopWalkSound()
    {
        if (SFXSource.isPlaying)
        {
            SFXSource.Stop();
        }
    }

    
}
