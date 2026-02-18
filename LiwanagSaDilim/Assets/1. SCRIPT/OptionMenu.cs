using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Importante para sa Brightness/Contrast

public class OptionsMenu : MonoBehaviour
{
    [Header("--- AUDIO SETTINGS ---")]
    public Slider bgmSlider; // I-drag ang BGM Slider dito
    public Slider sfxSlider; // I-drag ang SFX Slider dito
    public SoundManager soundManager; 

    [Header("--- VISUAL SETTINGS ---")]
    public Slider brightnessSlider; // I-drag ang Brightness Slider dito
    public Slider contrastSlider;   // I-drag ang Contrast Slider dito
    public Volume globalVolume;     // I-drag ang object na may Volume component dito

    private ColorAdjustments colorAdjustments;

    void Start()
    {
        if (soundManager == null)
            soundManager = FindObjectOfType<SoundManager>();

        // --- 1. SETUP AUDIO SLIDERS ---
        float savedBGM = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFX_Volume", 1f);

        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(savedBGM);
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(savedSFX);
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        // --- 2. SETUP VISUAL SLIDERS (URP) ---
        if (globalVolume != null)
        {
            // Kunin ang ColorAdjustments mula sa Volume Profile
            globalVolume.profile.TryGet(out colorAdjustments);
        }

        // Default value ay 0 para sa Brightness (Post Exposure) at Contrast
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0f); 
        float savedContrast = PlayerPrefs.GetFloat("Contrast", 0f);

        if (brightnessSlider != null)
        {
            brightnessSlider.SetValueWithoutNotify(savedBrightness);
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
            ApplyBrightness(savedBrightness);
        }
        if (contrastSlider != null)
        {
            contrastSlider.SetValueWithoutNotify(savedContrast);
            contrastSlider.onValueChanged.AddListener(OnContrastChanged);
            ApplyContrast(savedContrast);
        }
    }

    // --- AUDIO FUNCTIONS ---
    public void OnBGMSliderChanged(float value)
    {
        if (soundManager != null) soundManager.SetBGMVolume(value);
    }

    public void OnSFXSliderChanged(float value)
    {
        if (soundManager != null) soundManager.SetSFXVolume(value);
    }

    // --- VISUAL FUNCTIONS ---
    public void OnBrightnessChanged(float value)
    {
        ApplyBrightness(value);
        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
    }

    public void OnContrastChanged(float value)
    {
        ApplyContrast(value);
        PlayerPrefs.SetFloat("Contrast", value);
        PlayerPrefs.Save();
    }

    private void ApplyBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(value);
        }
    }

    private void ApplyContrast(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.contrast.Override(value);
        }
    }
}