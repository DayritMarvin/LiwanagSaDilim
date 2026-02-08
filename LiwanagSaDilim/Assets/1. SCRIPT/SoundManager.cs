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
        // "1" means ON, "0" means OFF. Default is 1.
        bool bgmOn = PlayerPrefs.GetInt("BGM_Active", 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt("SFX_Active", 1) == 1;

        BGMSource.mute = !bgmOn; // Kung bgmOn is true, mute is false
        SFXSource.mute = !sfxOn;

        // 2. Play Background Music
        if (background != null)
        {
            BGMSource.clip = background;
            BGMSource.loop = true; // Siguraduhing naka-loop ang BGM
            BGMSource.Play();
        }
    }

    // --- NEW: FUNCTION PARA SA TOGGLES ---
    
    public void ToggleBGM(bool isOn)
    {
        BGMSource.mute = !isOn;
        PlayerPrefs.SetInt("BGM_Active", isOn ? 1 : 0); // Save sa memory
        PlayerPrefs.Save();
    }

    public void ToggleSFX(bool isOn)
    {
        SFXSource.mute = !isOn;
        PlayerPrefs.SetInt("SFX_Active", isOn ? 1 : 0); // Save sa memory
        PlayerPrefs.Save();
    }

    // --- EXISTING LOGIC MO (Walang binago sa function, pero controlled na ng mute) ---

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayJumpSound()
    {
        if (SFXSource != null && jump != null)
        {
            // Note: PlayOneShot mas maganda para hindi maputol ang walk sound, 
            // pero stick muna tayo sa logic mo kung ito gusto mo.
            SFXSource.PlayOneShot(jump); 
        }
    }

    public void PlayWalkSound()
    {
        // Play lang kung hindi pa nagpe-play ang walk para iwas overlapping
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
            SFXSource.loop = false; // Reset loop
            SFXSource.clip = null;
        }
    }
}