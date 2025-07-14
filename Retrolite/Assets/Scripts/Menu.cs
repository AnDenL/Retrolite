using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public static float timeScale = 1;
    public static bool isPaused;

    [SerializeField] GameObject menu, selected;

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
            if (isPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        Player.canInteract = false;
        menu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(selected);
    }

    public void ResumeGame()
    {
        Time.timeScale = timeScale;
        Player.canInteract = true;
        menu.SetActive(false);
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
