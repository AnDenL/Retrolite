using System;
using System.Collections;
using UnityEngine;

public class BloodFountain : MonoBehaviour
{
    public TextMesh text;
    public GameObject[] Reward;
    public GameObject panel;
    public int Cost = 1;
    public ParticleSystem particles;
    private bool isActive;
    private void Start() 
    {  
        text.text = Convert.ToString(Cost);
        isActive = false;
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
            if (Input.GetKeyDown(KeyCode.E) && !isActive)
            {
                collision.GetComponent<Health>().SetHealth(1);
                StartCoroutine(ActivateFountain(collision.gameObject));
            } 
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player") {
            panel.SetActive(false);
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
        switch(UnityEngine.Random.Range(0,4)){
            case 0:
                Instantiate(Reward[UnityEngine.Random.Range(0,Reward.Length)], transform.parent.GetChild(0));
                break;
            case 1:
                SanitySystem sanity = player.GetComponent<SanitySystem>();
                sanity.Sanity += 1000f;
                break;
            default:
                Money m = player.GetComponent<Money>();
                m.money += UnityEngine.Random.Range(12, 25);
                break;
        }
    }
}