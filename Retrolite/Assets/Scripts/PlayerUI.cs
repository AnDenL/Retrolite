using TMPro;
using TMPro.Examples;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] TMP_Text healthText;
    [SerializeField] VertexJitter shakingText;
    [SerializeField] Slider healthBar, backBar;
    [SerializeField] Image healthBarFill;
    [SerializeField] LowHealthVolume effect;
    [SerializeField] Color healthBarColor;
    [SerializeField] Color lowHealthColor;

    [Header("Resources UI")]
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text codeText;

    private float healthPercent;

    private Coroutine healthBarAnimationCoroutine, healthTextCoroutine, moneyCoroutine, bitsCoroutine;

    private void Awake()
    {
        Player player = FindAnyObjectByType<Player>();

        player.OnHealthChanged += UpdateHealthUI;
        player.OnMoneyChange += UpdateMoneyText;
        player.OnBitsChange += UpdateBitsText;
    }

    public void UpdateMoneyText(int money)
    {
        bitsCoroutine = StartCoroutine(TextAnimation(moneyText, int.Parse(moneyText.text), money));
    }

    public void UpdateBitsText(int code)
    {
        bitsCoroutine = StartCoroutine(TextAnimation(codeText, int.Parse(codeText.text), code));
    }

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthPercent = currentHealth / maxHealth;
        healthBarFill.fillAmount = healthPercent;
        if (healthPercent < 0.6f)
        {
            effect.intensity = 1 - healthPercent - 0.3f;
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
            shakingText.CurveScale = 8f / (healthPercent * 6f + 0.3f);
        }
        else
        {
            shakingText.enabled = false;
        }

        if (healthBarAnimationCoroutine != null) StopCoroutine(healthBarAnimationCoroutine);
        if (healthTextCoroutine != null) StopCoroutine(healthTextCoroutine);
        healthBarAnimationCoroutine = StartCoroutine(HealthBarAnimation(currentHealth, maxHealth));
        healthTextCoroutine = StartCoroutine(HealthTextAnimation(healthText, healthBar.value * maxHealth, currentHealth, maxHealth));
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
            effect.intensity = 1 - bar2.value - 0.3f;
            healthBarFill.color = Color.Lerp(lowHealthColor, healthBarColor, bar2.value);
            yield return null;
        }
    }

    private IEnumerator TextAnimation(TMP_Text label, int start, int end)
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 2;
            label.text = "" + (int)Mathf.Lerp(start, end, t);
            yield return null;
        }

        label.text = end.ToString();
    }

    private IEnumerator HealthTextAnimation(TMP_Text label, float start, float end, float max)
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 2;
            float health = (float)Math.Round(Mathf.Lerp(start, end, t), 1);
            label.text = $"{Math.Round(health, 1)}/{max}";
            yield return null;
        }

        healthText.text = $"{Math.Round(end, 1)}/{max}";
    }
}
