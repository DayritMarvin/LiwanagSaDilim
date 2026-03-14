using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource BGMSource;
    [SerializeField] AudioSource SFXSource;

    [Header("--------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip jump;
    public AudioClip walk;
    public AudioClip firefly;
    public AudioClip push;

    private void Start()
    {
        // 1. I-apply ang saved settings pag-start ng game
        // Float na ang gagamitin natin para sa Slider (0.0f hanggang 1.0f)
        float bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);

        BGMSource.volume = bgmVolume; 
        SFXSource.volume = sfxVolume;

        // 2. Play Background Music
        if (background != null)
        {
            BGMSource.clip = background;
            BGMSource.loop = true; 
            BGMSource.Play();
        }
    }

    // --- BAGO: FUNCTION PARA SA SLIDERS ---
    
    public void SetBGMVolume(float volume)
    {
        BGMSource.volume = volume;
        PlayerPrefs.SetFloat("BGM_Volume", volume); // Save sa memory
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
        PlayerPrefs.SetFloat("SFX_Volume", volume); // Save sa memory
        PlayerPrefs.Save();
    }

    // --- EXISTING LOGIC MO ---

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayJumpSound()
    {
        if (SFXSource != null && jump != null)
        {
            SFXSource.PlayOneShot(jump); 
        }
    }

    public void PlayWalkSound()
    {
        if (SFXSource.clip != walk || !SFXSource.isPlaying)
        {
            SFXSource.clip = walk;
            SFXSource.loop = true; 
            SFXSource.Play();
        }
    }

    public void StopWalkSound()
    {
        if (SFXSource.clip == walk && SFXSource.isPlaying)
        {
            SFXSource.Stop();
            SFXSource.loop = false; 
            SFXSource.clip = null;
        }
    }
}