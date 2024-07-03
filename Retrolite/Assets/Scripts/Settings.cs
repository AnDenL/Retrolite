using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Settings : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider _masterVolume;
    [SerializeField] private Slider _musicVolume;
    [SerializeField] private Slider _effectsVolume;
    [SerializeField] private AudioMixerGroup _mixer;
    [Header("Graphic")]
    [SerializeField] private Toggle _fullscreen;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private Slider _brightness;
    [SerializeField] private VolumeProfile _brightnessProfile;
    private Resolution[] _resolutions;
    private ColorAdjustments Exposure;

    private void Start()
    {
        _brightnessProfile.TryGet<ColorAdjustments>(out Exposure);
        _resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        _resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + "x" + _resolutions[i].height + " " + _resolutions[i].refreshRateRatio + "Hz";
            options.Add(option);
            if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }
    #region SettingsMethods
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Master", _masterVolume.value);
        PlayerPrefs.SetFloat("Music", _musicVolume.value);
        PlayerPrefs.SetFloat("Effects", _effectsVolume.value);
        PlayerPrefs.SetInt("Resolution", _resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("Brightness", _brightness.value);
    }

    private void LoadSettings(int currentResolutionIndex)
    {
        _fullscreen.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen", 1));
        _resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", currentResolutionIndex);
        
        _masterVolume.value = PlayerPrefs.GetFloat("Master", 0.5f);
        ChangeMasterVolume(_masterVolume.value);
        _musicVolume.value = PlayerPrefs.GetFloat("Music", 1);
        ChangeMusicVolume(_musicVolume.value);
        _effectsVolume.value = PlayerPrefs.GetFloat("Effects", 1);
        ChangeEffectsVolume(_effectsVolume.value);

        _brightness.value = PlayerPrefs.GetFloat("Brightness", 0.5f);
        SetBrightness(_brightness.value);
    }
    #endregion
    #region Audio
    public void ChangeMasterVolume(float value) => _mixer.audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, Mathf.Log10(value) * 20f));
    public void ChangeMusicVolume(float value) => _mixer.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, Mathf.Log10(value) * 20f));
    public void ChangeEffectsVolume(float value) => _mixer.audioMixer.SetFloat("EffectsVolume", Mathf.Lerp(-80, 0, Mathf.Log10(value) * 20f));
    #endregion
    #region Graphics
    public void SetBrightness(float Brightness) => Exposure.postExposure.value = Mathf.Lerp(0, 1, Brightness);
    public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    #endregion
}
