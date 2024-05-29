using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextLevel : MonoBehaviour
{
    public int LoadedLevel;
    public GameObject Button;
    public bool nextlevel, savePlayer;
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
        if (savePlayer){
            GameObject Player = GameObject.Find("Player");
            DataHolder.money = Player.GetComponent<Money>().money;
            DataHolder.Health = Player.GetComponent<Health>().healthPoint;
            DataHolder.MaxHealth = Player.GetComponent<Health>().maxHealthPoint;
            DataHolder.lifes = Player.GetComponent<Health>().lifes;
            DataHolder.Sanity = Player.GetComponent<SanitySystem>().Sanity;
            WeaponsList guns = Player.GetComponent<WeaponsList>();
            DataHolder.FirstWeapon = guns.firstGun;
            DataHolder.SecondWeapon = guns.secondGun;
        }
        if (nextlevel){
            SceneManager.LoadScene(currentLevel + 1);
        } 
        else SceneManager.LoadScene(LoadedLevel);
    }
    private void BlackScreen() => Fade.SetTrigger("End");
}
