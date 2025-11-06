using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Creatures;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public static float TimeSpeed { get; private set; } = 1;
    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject selected;
    [SerializeField] private GameObject console;
    [SerializeField] private AudioMixer mixer;  

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menu)
        {
            IsPaused = !IsPaused;
            console.SetActive(false);

            if (IsPaused) PauseGame(menu);
            else ResumeGame(menu);
        }
        else if ((Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.BackQuote)) && LuaApi.UseLua)
        {
            console.SetActive(!console.activeSelf);
            menu.SetActive(false);

            if (console.activeInHierarchy) PauseGame(console);
            else ResumeGame(console);
        }
    }

    public void PauseGame(GameObject panel)
    {
        Time.timeScale = 0;
        PlayerController.CanInteract = false;
        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(selected);
        mixer.SetFloat("AudioLowPass", 600);
    }

    public void ResumeGame(GameObject panel)
    {
        Time.timeScale = TimeSpeed;
        PlayerController.CanInteract = true;
        panel.SetActive(false);
        mixer.SetFloat("AudioLowPass", 22000);
    }

    public void LoadLevel(int levelIndex) => SceneManager.LoadScene(levelIndex);
    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void QuitGame() => Application.Quit();
}
