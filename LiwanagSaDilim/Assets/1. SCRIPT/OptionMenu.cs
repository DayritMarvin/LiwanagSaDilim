using UnityEngine;
using UnityEngine.UI; // Importante para sa Toggle

public class OptionsMenu : MonoBehaviour
{
    public Toggle bgmToggle; // I-drag ang BGM Checkbox dito
    public Toggle sfxToggle; // I-drag ang SFX Checkbox dito
    
    public SoundManager soundManager; // I-drag ang SoundManager object dito

    void Start()
    {
        // 1. Hanapin ang SoundManager kung hindi na-drag
        if (soundManager == null)
            soundManager = FindObjectOfType<SoundManager>();

        // 2. Kunin ang saved data para i-update ang UI visual (Check/Uncheck)
        bool isBGMOn = PlayerPrefs.GetInt("BGM_Active", 1) == 1;
        bool isSFXOn = PlayerPrefs.GetInt("SFX_Active", 1) == 1;

        // 3. I-set ang Toggles visual nang hindi nagti-trigger ng sound glitch
        // SetIsOnWithoutNotify para lang mabago ang itsura pero di tatawagin ang function agad
        if (bgmToggle != null) bgmToggle.SetIsOnWithoutNotify(isBGMOn);
        if (sfxToggle != null) sfxToggle.SetIsOnWithoutNotify(isSFXOn);

        // 4. Mag-subscribe sa events (Para pag pinindot, gagana)
        if (bgmToggle != null) bgmToggle.onValueChanged.AddListener(OnBGMToggleChanged);
        if (sfxToggle != null) sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
    }

    // Ito ang tatawagin kapag pinindot ang BGM Checkbox
    public void OnBGMToggleChanged(bool isOn)
    {
        if (soundManager != null) soundManager.ToggleBGM(isOn);
    }

    // Ito ang tatawagin kapag pinindot ang SFX Checkbox
    public void OnSFXToggleChanged(bool isOn)
    {
        if (soundManager != null) soundManager.ToggleSFX(isOn);
    }
}