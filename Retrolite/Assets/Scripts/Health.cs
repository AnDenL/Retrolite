using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public float healthPoint, maxHealthPoint, destroyDelay;
    public int lifes = 0;
    public TextMesh text;
    public Text playerText;
    public Animator animator, animator2;
    public AudioClip takeHitSound, deathSound;
    public bool canDie;
    public bool isDead;
    public int LoadLevel;
    public float InvisibleTime = 0.1f;
    public bool isInvincible = false;
    public int Features = 0;
    public GameObject RevivalEffect; 

    private AudioSource sound;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
        if (Features != 2) text.text = Convert.ToString(healthPoint);
        else playerText.text = "Health: " + Convert.ToString(Math.Round(healthPoint, 2)) + "/" + Convert.ToString(maxHealthPoint);
    }

    public void Heal(float H)
    {
        healthPoint += H;
        if (healthPoint > maxHealthPoint) healthPoint = maxHealthPoint;
        if(Features == 2){
            playerText.text = "Health: " + Convert.ToString(Math.Round(healthPoint, 2)) + "/" + Convert.ToString(maxHealthPoint);
            if (lifes != 0)playerText.text += " x" + Convert.ToString(lifes);
        }
        else text.text = Convert.ToString(Math.Round(healthPoint, 2));
        GameObject Effect = Instantiate(RevivalEffect, transform.parent);
        Effect.transform.position = transform.position;
        Effect.transform.parent = null;
    }

    public bool SetHealth(float damage, float pureDamage = 0,float sanityDamage = 0)
    {
        switch(Features){
            case 0:
                TakeDamage(damage + pureDamage);
                return isDead;
            case 1:
                TakeDamage(-damage - pureDamage);
                return isDead;
            case 2:
                bool damageDealt = !isInvincible;
                if(damage != 0)PlayerTakeDamage(damage);
                GetComponent<SanitySystem>().Sanity -= sanityDamage;
                return damageDealt;
            case 3:
                GolemHead Head = GetComponent<GolemHead>();
                if(Head.charges != 0) Head.LaserShoot();
                TakeDamage(damage + pureDamage);
                return isDead;
            case 4:
                GolemBody Body = GetComponent<GolemBody>();
                if(Body.charges != 0) {
                    Body.Shield();
                    if(pureDamage != 0)TakeDamage(pureDamage);
                }
                else TakeDamage(damage);
                return isDead;
            default:
                return false;

        }
    }

    private bool TakeDamage(float damage)
    {
        if (!float.IsNaN(damage))
        {
            healthPoint -= damage;

            if (healthPoint <= 0)
            {
                if(canDie)Death();
            }

            else
            {
                if(sound != null)
                {
                    sound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                    sound.PlayOneShot(takeHitSound);
                }
                
                if (animator != null)
                    animator.SetTrigger("TakeHit");
            }

            if (healthPoint > maxHealthPoint) healthPoint = maxHealthPoint;

            text.text = Convert.ToString(Math.Round(healthPoint, 2));

            StartCoroutine(InvincibilityTimer());
        }
        return isDead;
    }
    
    private void PlayerTakeDamage(float damage)
    {
        if (!float.IsNaN(damage) && !isInvincible)
        {
            healthPoint -= damage;

            if (healthPoint <= 0)
            {
                PlayerDeath();
            }
            else
            {
                sound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                sound.PlayOneShot(takeHitSound);
                if (animator != null)animator.SetTrigger("TakeHit");
            }

            if (healthPoint > maxHealthPoint) healthPoint = maxHealthPoint;

            playerText.text = "Health: " + Convert.ToString(Math.Round(healthPoint, 2)) + "/" + Convert.ToString(maxHealthPoint);
            if (lifes != 0)playerText.text += " x" + Convert.ToString(lifes);

            StartCoroutine(InvincibilityTimer());
        }
    }

    private void Death()
    {
        if (!isDead)
        {
            sound.PlayOneShot(deathSound);
            isDead = true;
            animator.SetTrigger("Death");

            if (canDie && destroyDelay >= 0)Destroy(gameObject, destroyDelay);
        }
    }

    private void PlayerDeath()
    {
        if (!isDead && lifes == 0)
        {
            sound.PlayOneShot(deathSound);
            isDead = true;
            animator.SetTrigger("Death");

            Time.timeScale = 0.2f;
            animator2.SetTrigger("End");
            Invoke("LoadScene", 0.3f);

        }
        else if (lifes > 0)
        {
            healthPoint = maxHealthPoint;
            GetComponent<SanitySystem>().Sanity = 10000;
            lifes--;
            GameObject Effect = Instantiate(RevivalEffect, transform.parent);
            Effect.transform.position = transform.position;
            Effect.transform.parent = null;
        }
    }
    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvisibleTime);
        isInvincible = false;
    }

    private void LoadScene() => SceneManager.LoadScene(LoadLevel);
}