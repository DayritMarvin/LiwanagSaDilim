using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource BGMSource;
    [SerializeField] public AudioSource SFXSource;

    [Header("--------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip jump;
    public AudioClip walk;
    public AudioClip firefly;
    public AudioClip push;

    // --- MAGDAGDAG TAYO NG VOLUME CONTROLS SA INSPECTOR ---
    [Header("--------- Specific SFX Volumes ---------")]
    [Range(0f, 1f)] public float jumpVolume = 0.5f; // Pwedeng baguhin sa Unity Inspector
    [Range(0f, 1f)] public float walkVolume = 0.5f; // Pwedeng baguhin sa Unity Inspector
    
    private float defaultSFXVolume; // Para matandaan ang original SFX volume

    private void Start()
    {
        float bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);

        BGMSource.volume = bgmVolume; 
        SFXSource.volume = sfxVolume;
        defaultSFXVolume = sfxVolume; // I-save natin ito para sa walk sound mamaya

        if (background != null)
        {
            BGMSource.clip = background;
            BGMSource.loop = true; 
            BGMSource.Play();
        }
    }

    public void SetBGMVolume(float volume)
    {
        BGMSource.volume = volume;
        PlayerPrefs.SetFloat("BGM_Volume", volume); 
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
        defaultSFXVolume = volume; // I-update din natin yung default kapag nagbago via slider
        PlayerPrefs.SetFloat("SFX_Volume", volume); 
        PlayerPrefs.Save();
    }

    public void PlaySFX(AudioClip clip)
    {
        // Gagamitin ang buong SFX volume para sa normal clips tulad ng firefly o push
        SFXSource.PlayOneShot(clip);
    }

    // --- UPDATED: PlayJumpSound ---
    public void PlayJumpSound()
    {
        if (SFXSource != null && jump != null)
        {
            // Babawasan natin ang lakas base sa 'jumpVolume' pero susunod pa rin siya sa main SFX slider
            SFXSource.PlayOneShot(jump, jumpVolume); 
        }
    }

    // --- UPDATED: PlayWalkSound ---
    public void PlayWalkSound()
    {
        if (SFXSource.clip != walk || !SFXSource.isPlaying)
        {
            SFXSource.clip = walk;
            SFXSource.loop = true; 
            
            // Pansamantalang ibababa ang master volume ng SFX Source para lang sa walk
            // Kailangan natin i-multiply sa default para kung 50% yung slider sa settings, 
            // tapos 50% yung walkVolume mo, magiging 25% na lang talaga yung lakas.
            SFXSource.volume = defaultSFXVolume * walkVolume; 
            
            SFXSource.Play();
        }
    }

    // --- UPDATED: StopWalkSound ---
    public void StopWalkSound()
    {
        if (SFXSource.clip == walk && SFXSource.isPlaying)
        {
            SFXSource.Stop();
            SFXSource.loop = false; 
            SFXSource.clip = null;
            
            // IBABALIK natin sa normal SFX volume kapag huminto na sa paglakad
            SFXSource.volume = defaultSFXVolume; 
        }
    }
}