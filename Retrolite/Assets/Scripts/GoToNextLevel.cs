using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextLevel : MonoBehaviour
{
    public int[] LoadedLevel;
    public GameObject Button;
    private bool PlayerIsHere;
    private Animator Fade;
    private Animator Player;
    private Animator animator;
    private void Start() 
    {
        Player = GameObject.Find("Player").GetComponent<Animator>();
        Fade = GameObject.Find("Fade").GetComponent<Animator>();
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Button.SetActive(true);
            PlayerIsHere = true;
        }  
    }
    private void Update()
    {
        if (Input.GetAxis("Action") != 0 && PlayerIsHere)
        {
            GetComponent<AudioSource>().enabled = true;
            Player.SetTrigger("End");
            animator.SetTrigger("End");
            Invoke("BlackScreen",0.5f);
            Invoke("UnLockLevel",1f);
        }  
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Button.SetActive(false);
            PlayerIsHere = false;
        }  
    }
    private void UnLockLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        if(SavingSystem.instance != null) SavingSystem.SaveRun();
        SceneManager.LoadScene(LoadedLevel[Random.Range(0, LoadedLevel.Length)]);
    }
    private void BlackScreen() => Fade.SetTrigger("End");
}
