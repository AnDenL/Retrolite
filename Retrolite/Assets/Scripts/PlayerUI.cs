using TMPro;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField]
    private TMP_Text healthText;
    [SerializeField]
    private ShakingText shakingText;
    [SerializeField]
    private Slider healthBar, backBar;
    [SerializeField]
    private Image healthBarFill;
    [SerializeField]
    private LowHealthVolume effect;
    [SerializeField]
    private Color healthBarColor;
    [SerializeField]
    private Color lowHealthColor;

    [Header("Resources UI")]
    [SerializeField]
    private TMP_Text moneyText;
    [SerializeField]
    private TMP_Text codeText;

    private Vector3 originalTextPos;
    private float healthPercent;

    private Coroutine healthBarAnimationCoroutine;

    private void Start()
    {
        originalTextPos = transform.localPosition;
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }

    public void UpdateCodeText(int code)
    {
        codeText.text = code.ToString();
    }

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthPercent = currentHealth / maxHealth;
        healthBarFill.fillAmount = healthPercent;
        if (healthPercent < 0.6f)
        {
            effect.intensity = (1 - healthPercent) - 0.3f;
            effect.enabled = true;
            effect.volume.enabled = true;
        }
        else
        {
            effect.intensity = 0;
            effect.enabled = true;
            effect.volume.enabled = false;
        }

        if (healthPercent < 0.5f)
        {
            shakingText.enabled = true;
            shakingText.shakeMagnitude = 0.2f / healthPercent + 0.5f;
            shakingText.shakeSpeed = 2f / healthPercent + 0.5f;
        }
        else
        {
            shakingText.enabled = false;
        }

        if (healthBarAnimationCoroutine != null) StopCoroutine(healthBarAnimationCoroutine);
        healthBarAnimationCoroutine = StartCoroutine(HealthBarAnimation(currentHealth, maxHealth));

        healthText.text = $"{Math.Round(currentHealth, 1)}/{maxHealth}";
    }

    private IEnumerator HealthBarAnimation(float currentHealth, float maxHealth)
    {
        float t = 1;
        float v = healthBar.value;
        float currentHealthPercentage = currentHealth / maxHealth;
        Slider bar1, bar2;
        if (currentHealthPercentage < v)
        {
            bar1 = healthBar;
            bar2 = backBar;
        }
        else
        {
            bar1 = backBar;
            bar2 = healthBar;
        }


        bar1.value = currentHealthPercentage;

        while (t > 0)
        {
            t -= Time.deltaTime * 2;
            bar2.value = Mathf.Lerp(currentHealthPercentage, v, t * t);
            healthBarFill.color = Color.Lerp(lowHealthColor, healthBarColor, healthBar.value);
            yield return null;
        }
    }
}
