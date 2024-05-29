using System;
using System.Collections;
using UnityEngine;

public class Fountain : MonoBehaviour
{
    public TextMesh text;
    public GameObject[] Reward;
    public GameObject panel;
    public int defaultCost = 10;
    public ParticleSystem particles;
    public AudioClip sound;

    private AudioSource Fx;
    private int cost;
    private bool isActive, playerIsHere;
    private GameObject Player;

    private void Start() 
    {  
        cost = defaultCost;
        text.text = Convert.ToString(cost);
        isActive = false;
        Fx = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player") {
            panel.SetActive(true);
            playerIsHere = true;
            Player = collision.gameObject;
        }
    }
    void Update()
    {
        if(playerIsHere)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isActive)
            {
                bool b = Player.GetComponent<Money>().Buy(cost);
                if (b) {
                    StartCoroutine(ActivateFountain(Player));
                }
            } 
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player") {
            panel.SetActive(false);
            playerIsHere = false;
        }
    }

    IEnumerator ActivateFountain(GameObject player)
    {
        isActive = true;
        particles.Play();
        TakePurchase(player);
        yield return new WaitForSeconds(0.5f);
        isActive = false;
    }

    void TakePurchase(GameObject player)
    {
        cost += defaultCost / 3;
        text.text = Convert.ToString(cost);
        switch(UnityEngine.Random.Range(0,4)){
            case 0:
                Instantiate(Reward[UnityEngine.Random.Range(0,Reward.Length)], transform.parent.GetChild(0));
                Fx.PlayOneShot(sound);
                break;
            case 1:
                Health hp = player.GetComponent<Health>();
                if (hp.maxHealthPoint != hp.healthPoint) {
                    hp.Heal(hp.maxHealthPoint); 
                }
                Fx.PlayOneShot(sound);
                break;
            default :
                Fx.PlayOneShot(sound);
                break;
        }
    }
}