using System;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private int cost;
    private int purchase;
    public AudioClip DrinkSound;
    public TextMesh text, description;
    public Sprite[] Image;
    public GameObject panel;
    public bool needToBuy = true;
    private void Start() 
    {  
        SpriteRenderer Sprite = GetComponent<SpriteRenderer>();
        purchase = (UnityEngine.Random.Range(0,6));
        Sprite.sprite = Image[purchase];
        switch(purchase){
            case 0:
                cost = 20;
                description.text = "Heals you for 5 health points";
                break;
            case 1:
                cost = 35;
                description.text = "Heals you for 10 health points";
                break;
            case 2:
                cost = 30;
                description.text = "Increases your maximum health for 2 health points";
                break;
            case 3:
                cost = 40;
                description.text = "Protects you from insanity";
                break;
            case 4:
                cost = 100;
                description.text = "Brings you back to life if you die";
                break;
            case 5:
                cost = 10;
                description.text = "Better not to drink it...";
                break;
        }
        if(!needToBuy) cost = 0;
        if (cost != 0)text.text = Convert.ToString(cost);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player") {
            panel.SetActive(true);
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player") 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                bool b = collision.GetComponent<Money>().Buy(cost);
                if (b) TakePurchase(collision.gameObject);
            } 
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player") {
            panel.SetActive(false);
        }
    }
    void TakePurchase(GameObject Player)
    {
        Player.GetComponent<AudioSource>().PlayOneShot(DrinkSound);
        switch(purchase){
            case 0:
                Player.GetComponent<Health>().SetHealth(-5);
                break;
            case 1:
                Player.GetComponent<Health>().SetHealth(-10);
                break;
            case 2:
                Player.GetComponent<Health>().maxHealthPoint += 2;
                Player.GetComponent<Health>().SetHealth(-2);
                break;
            case 3:
                Player.GetComponent<Health>().SetHealth(0,-3000);
                break;
            case 4:
                Player.GetComponent<Health>().lifes += 1;
                Player.GetComponent<Health>().SetHealth(-2);
                break;
            case 5:
                Player.GetComponent<Health>().SetHealth(UnityEngine.Random.Range(-6,4), 100);
                break;
        }
        Destroy(gameObject);
    }
}
