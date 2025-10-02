using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public static float timeScale = 1;
    public static bool isPaused;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject selected;
    [SerializeField] private GameObject console;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menu)
        {
            isPaused = !isPaused;
            console.SetActive(false);
            if (isPaused) PauseGame(menu);
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
        Player.canInteract = false;
        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(selected);
    }

    public void ResumeGame(GameObject panel)
    {
        Time.timeScale = timeScale;
        Player.canInteract = true;
        panel.SetActive(false);
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
