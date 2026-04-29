using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OptionsMenu : MonoBehaviour
{
    [Header("--- AUDIO SETTINGS ---")]
    public Slider bgmSlider; 
    public Slider sfxSlider; 
    public SoundManager soundManager; 

    [Header("--- VISUAL SETTINGS ---")]
    public Slider brightnessSlider; 
    public Slider contrastSlider;   
    public Volume globalVolume;     

    [Header("--- VALUE TEXT DISPLAYS (0-100) ---")]
    public TMP_Text bgmTextValue;
    public TMP_Text sfxTextValue;
    public TMP_Text brightnessTextValue;
    public TMP_Text contrastTextValue;

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
            UpdateTextValue(bgmTextValue, savedBGM, 0f, 1f);
        }
        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(savedSFX);
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
            UpdateTextValue(sfxTextValue, savedSFX, 0f, 1f);
        }

        // --- 2. SETUP VISUAL SLIDERS (URP) ---
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out colorAdjustments);
        }

        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0f); 
        float savedContrast = PlayerPrefs.GetFloat("Contrast", 0f);

        if (brightnessSlider != null)
        {
            brightnessSlider.SetValueWithoutNotify(savedBrightness);
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
            ApplyBrightness(savedBrightness);
            UpdateTextValue(brightnessTextValue, savedBrightness, -2f, 2f);
        }
        if (contrastSlider != null)
        {
            contrastSlider.SetValueWithoutNotify(savedContrast);
            contrastSlider.onValueChanged.AddListener(OnContrastChanged);
            ApplyContrast(savedContrast);
            UpdateTextValue(contrastTextValue, savedContrast, -50f, 50f);
        }
    }

    // --- AUDIO FUNCTIONS ---
    public void OnBGMSliderChanged(float value)
    {
        if (soundManager != null) soundManager.SetBGMVolume(value);
        UpdateTextValue(bgmTextValue, value, 0f, 1f);
    }

    public void OnSFXSliderChanged(float value)
    {
        if (soundManager != null) soundManager.SetSFXVolume(value);
        UpdateTextValue(sfxTextValue, value, 0f, 1f);
    }

    // --- VISUAL FUNCTIONS ---
    public void OnBrightnessChanged(float value)
    {
        ApplyBrightness(value);
        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
        UpdateTextValue(brightnessTextValue, value, -2f, 2f);
    }

    public void OnContrastChanged(float value)
    {
        ApplyContrast(value);
        PlayerPrefs.SetFloat("Contrast", value);
        PlayerPrefs.Save();
        UpdateTextValue(contrastTextValue, value, -50f, 50f);
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

    private void UpdateTextValue(TMP_Text textComponent, float value, float min, float max)
    {
        if (textComponent != null)
        {
            float percentage = Mathf.InverseLerp(min, max, value) * 100f;
            textComponent.text = Mathf.RoundToInt(percentage).ToString(); 
        }
    }
}