using UnityEngine;

public class Pc : MonoBehaviour
{
    public GameObject Ui;
    public PlayerMove Player;
    private float pSpeed;
    private bool opened = false ,IsHere = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsHere = true;
        }  
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && IsHere)
        {
            if(!opened){
                opened = true;
                pSpeed = Player.speed;
                Player.speed = 0;
                Ui.SetActive(true);  
            }
            else
            {
                opened = false;
                Player.speed = pSpeed;
                Ui.SetActive(false);  
            }
        }  
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsHere = false;
        }  
    }
}
