using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Settings : MonoBehaviour
{
    private ColorAdjustments Exposure;
    public Camera Game,UiCamera,Water;
    public AudioMixerGroup Mixer;
    public Toggle pixelEffect;
    public Dropdown resolutionDropdown;
    public Slider EffectSlider, MusicSlider, MasterSlider, BrightnessSlider;
    public Resolution[] resolutions;
    public UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera pixelPerfectCamera;
    public RenderTexture water,UI;
    public VolumeProfile BrightnessProfile;
    public RawImage image;
    public Material material;

    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
        Invoke("t",0.1f);
    }
    private void t(){
        SetResolution(resolutionDropdown.value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void PixelEffect(bool pixelEffect)
    {
        pixelPerfectCamera.enabled = pixelEffect;
        SetResolution(resolutionDropdown.value);
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        float currentAspect = Screen.height / (float)Screen.width;
        if (currentAspect < 0.5625f)
        {
            int ResolutionY = Convert.ToInt32(171 * (currentAspect / 0.5625f));
            pixelPerfectCamera.refResolutionY = ResolutionY;
        }
        else
        {
            int ResolutionX = Convert.ToInt32(304 * (currentAspect / 0.5625f));
            pixelPerfectCamera.refResolutionX = ResolutionX;
        }
        if (water != null)
        {
            water = new RenderTexture(Screen.width, Screen.height, 0);
            Water.targetTexture = water;
            Water.orthographicSize = Game.orthographicSize;
            material.SetTexture("_RenderTexture", water);
        }
        UI = new RenderTexture(Screen.width, Screen.height, 0);
        UiCamera.targetTexture = UI;
        UiCamera.orthographicSize = Game.orthographicSize;
        image.texture = UI;
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetInt("PixelEffectPreference", Convert.ToInt32(pixelEffect.isOn));
        PlayerPrefs.SetFloat("MasterPreference", MasterSlider.value);
        PlayerPrefs.SetFloat("MusicPreference", MusicSlider.value);
        PlayerPrefs.SetFloat("EffectsPreference", EffectSlider.value);
        PlayerPrefs.SetFloat("BrightnessPreference", BrightnessSlider.value);
    }
    public void ChangeMasterVolume(float MasterVolume)
    {
        Mixer.audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-60, 0, MasterVolume));
    }
    public void ChangeMusicVolume(float Musicvolume)
    {
        Mixer.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-60, 0, Musicvolume));
    }
    public void ChangeEffectsVolume(float Effectsvolume)
    {
        Mixer.audioMixer.SetFloat("EffectsVolume", Mathf.Lerp(-60, 0, Effectsvolume));
    }
    public void ChangeBrightness(float Brightness)
    {
        BrightnessProfile.TryGet<ColorAdjustments>(out Exposure);
        Exposure.postExposure.value = Mathf.Lerp(0, 2, Brightness);
    }
    public void LoadSettings(int currentResolutionIndex)
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterPreference");
        float musicVolume = PlayerPrefs.GetFloat("MusicPreference");
        float effectVolume = PlayerPrefs.GetFloat("EffectsPreference");
        MasterSlider.value = masterVolume;
        MusicSlider.value = musicVolume;
        EffectSlider.value = effectVolume;
        Mixer.audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-60, 0, masterVolume));
        Mixer.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-60, 0, musicVolume));
        Mixer.audioMixer.SetFloat("EffectsVolume", Mathf.Lerp(-60, 0, effectVolume));
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;
        BrightnessSlider.value = PlayerPrefs.GetFloat("BrightnessPreference");
        ChangeBrightness(BrightnessSlider.value);
        SetResolution(resolutionDropdown.value);
        PixelEffect(Convert.ToBoolean(PlayerPrefs.GetInt("PixelEffectPreference")));
    }
}
