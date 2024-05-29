using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public class MainManu : MonoBehaviour
{
    [SerializeField] private GameObject _console;
    public static float _timeScale = 1;
    public GameObject PauseObj, blur;
    public bool menu;
    public Settings settings;
    public AudioSource HoverFX;
    public AudioMixerGroup Mixer;
    void Awake()
    {
        Time.timeScale = _timeScale;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menu) Pause();
            else Resume();  
        }
        if(Input.GetKeyDown(KeyCode.BackQuote)) 
        {
            bool active = _console.activeInHierarchy;
            Time.timeScale = Convert.ToInt32(active) * _timeScale;
            _console.SetActive(!active);
        }
    }
    public void HoverSound()
    {
        HoverFX.pitch = UnityEngine.Random.insideUnitCircle.magnitude * 0.1f + 0.9f;
        HoverFX.Play();
    }
    public void Resume()
    {
        settings.SaveSettings();
        Time.timeScale = _timeScale;
        PauseObj.SetActive(false);
        blur.SetActive(false);
        menu = false;
        settings.ChangeEffectsVolume(PlayerPrefs.GetFloat("EffectsPreference"));
        Mixer.audioMixer.SetFloat("MusicLowPass", 22000);
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        PauseObj.SetActive(true);
        blur.SetActive(true);
        menu = true;
        settings.ChangeEffectsVolume(0);
        Mixer.audioMixer.SetFloat("MusicLowPass", 600);
    }
    public void loadLevel(int levelIndex) => SceneManager.LoadScene(levelIndex);
    public void ExitGame() => Application.Quit();
}
