using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public static SettingsData settings;

    [SerializeField] Slider masterSlider, musicSlider, effectSlider;
    [SerializeField] AudioMixerGroup mixer;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullscreenToggle;

    private Resolution[] resolutions;

    private void Start()
    {
        settings ??= new();
        LoadSettings();
    }

    private void OnMasterSliderChanged(float value)
    {
        settings.MasterVolume = value;
        mixer.audioMixer.SetFloat("MasterVolume", NumToDecibel(value));
    }
    private void OnMusicSliderChanged(float value)
    {
        settings.MusicVolume = value;
        mixer.audioMixer.SetFloat("MusicVolume", NumToDecibel(value));
    }
    private void OnEffectSliderChanged(float value)
    {
        settings.EffectVolume = value;
        mixer.audioMixer.SetFloat("EffectVolume", NumToDecibel(value));
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        settings.Fullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    private void LoadSettings()
    {
        masterSlider.value = settings.MasterVolume;
        musicSlider.value = settings.MusicVolume;
        effectSlider.value = settings.EffectVolume;
        masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        effectSlider.onValueChanged.AddListener(OnEffectSliderChanged);

        resolutionDropdown.value = settings.ResolutionIndex;
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        fullscreenToggle.isOn = settings.Fullscreen;
        resolutions = Screen.resolutions;
        SetResolution(settings.ResolutionIndex);
        SetFullscreen(settings.Fullscreen);

        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = settings.ResolutionIndex;

        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;
    }

    public void Save() => settings.Save();

    public float NumToDecibel(float num) => Mathf.Log10(num) * 20;
}

public class SettingsData
{
    public float MasterVolume { get; set; }
    public float MusicVolume { get; set; }
    public float EffectVolume { get; set; }
    public int ResolutionIndex { get; set; }
    public bool Fullscreen { get; set; }
    public bool UseLua { get; set; }

    public SettingsData()
    {
        Load();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("EffectVolume", EffectVolume);
        PlayerPrefs.SetInt("ResolutionIndex", ResolutionIndex);
        PlayerPrefs.SetInt("Fullscreen", Fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("UseLua", UseLua ? 1 : 0);
    }

    public void Load()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        EffectVolume = PlayerPrefs.GetFloat("EffectVolume", 1f);
        ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        Fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        UseLua = PlayerPrefs.GetInt("UseLua", 0) == 1;
    }
}
