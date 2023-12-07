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
    public bool invertDamage = false;
    public GameObject RevivalEffect; 

    private AudioSource sound;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
        if (!CompareTag("Player")) text.text = Convert.ToString(healthPoint);
        else playerText.text = "Health: " + Convert.ToString(Math.Round(healthPoint, 2)) + "/" + Convert.ToString(maxHealthPoint);
    }

    public bool SetHealth(float damage, float sanityDamage = 0)
    {
        if (CompareTag("Player") && isInvincible) damage = -23.13f;
        else if(invertDamage) damage *= -1;
        if (!float.IsNaN(damage) && damage != -23.13f)
        {
            healthPoint -= damage;

            if (healthPoint <= 0)
            {
                Death();
            }
            else
            {
                sound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                sound.PlayOneShot(takeHitSound);
                if (animator != null)animator.SetTrigger("TakeHit");
            }

            if (healthPoint > maxHealthPoint) healthPoint = maxHealthPoint;
            if (!CompareTag("Player")) text.text = Convert.ToString(Math.Round(healthPoint, 2));
            else {
                playerText.text = "Health: " + Convert.ToString(Math.Round(healthPoint, 2)) + "/" + Convert.ToString(maxHealthPoint);
                if (lifes != 0)playerText.text += " x" + Convert.ToString(lifes);
                GetComponent<SanitySystem>().Sanity -= sanityDamage;
            }
            StartCoroutine(InvincibilityTimer());
        }
        return isDead;
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvisibleTime);
        isInvincible = false;
    }

    private void Death()
    {
        if (!isDead && lifes == 0)
        {
            sound.PlayOneShot(deathSound);
            isDead = true;
            animator.SetTrigger("Death");

            if (CompareTag("Player"))
            {
                Time.timeScale = 0.2f;
                animator2.SetTrigger("End");
                Invoke("LoadScene", 0.3f);
            }
            else
            {
                if (canDie)Destroy(gameObject, destroyDelay);
            }
        }
        else if (lifes > 0)
        {
            healthPoint = maxHealthPoint;
            GetComponent<SanitySystem>().Sanity = 10000;
            lifes--;
            GameObject Effect = Instantiate(RevivalEffect, transform.parent);
            Effect.transform.position = new Vector2(transform.position.x, transform.position.y);
            Effect.transform.parent = null;
        }
    }

    private void LoadScene() => SceneManager.LoadScene(LoadLevel);
}