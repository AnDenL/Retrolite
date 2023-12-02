using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public class MainManu : MonoBehaviour
{
    public GameObject PauseObj, blur;
    public bool menu;
    public Settings settings;
    public AudioSource HoverFX;
    public AudioMixerGroup Mixer;
    void Awake()
    {
        Time.timeScale = 1f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menu) Pause();
            else if (menu) Resume();  
        }
    }
    public void HoverSound()
    {
        HoverFX.pitch = Random.insideUnitCircle.magnitude * 0.1f + 0.9f;
        HoverFX.Play();
    }
    public void Resume()
    {
        settings.SaveSettings();
        Time.timeScale = 1f;
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
