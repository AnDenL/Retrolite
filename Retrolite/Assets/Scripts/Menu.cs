using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]private GameObject _menu;

    private bool _paused = false;

    private void Start()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        if(_menu == null) return;
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(_paused) Resume();
            else Pause();
        }
        if(Input.GetKeyDown(KeyCode.Tab) && !_paused) 
        {
            bool active = Game.Inventory.gameObject.transform.GetChild(0).gameObject.activeInHierarchy;
            Game.Paused = !active;
            Game.Inventory.gameObject.transform.GetChild(0).gameObject.SetActive(!active);
            Game.Inventory.SelectedItem = null;
        }
    }

    public void Pause()
    {
        Game.Inventory.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        _paused = true;
        Game.Paused = true;
        _menu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        _paused = false;
        Game.Paused = false;
        _menu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
