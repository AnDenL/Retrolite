using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMesh text;
    [SerializeField] private Transform healthSlider;
    [SerializeField] private SpriteRenderer stabilitySlider;
    [SerializeField] private Transform bg;
    [SerializeField] private Creature target;

    [Header("Scaling")]
    [SerializeField] private Vector2 uiScale = new Vector2(1f, 1f); // головний параметр

    private const float healthWidth = 1.1f;
    private const float healthHeight = 0.25f;

    private const float stabilityWidth = 0.625f;
    private const float stabilityHeight = 0.0625f;

    private void Start()
    {
        target.HealthComponent.OnHealthChanged += OnHealthChange;
        target.Corruption.OnCorrupting += OnStabilityChange;

        ApplyBaseScale();
        OnHealthChange(target.HealthComponent.Health, target.HealthComponent.MaxHealth);
        OnStabilityChange(target.Corruption.Stability);
    }

    private void ApplyBaseScale()
    {
        healthSlider.localScale = new Vector3(healthWidth * uiScale.x, healthHeight * uiScale.y, 1);
        bg.localScale = new Vector3(healthWidth * uiScale.x + 0.15f, healthHeight * uiScale.y + 0.15f, 1);
        stabilitySlider.transform.localScale = new Vector3(2f * uiScale.x / target.Corruption.MaxStability, 1f * uiScale.y, 1);
    }

    private void OnStabilityChange(int stability)
    {
        float segmentWidth = stabilityWidth * uiScale.x;
        stabilitySlider.size = new Vector2(segmentWidth * stability, stabilityHeight * uiScale.y);
    }

    private void OnHealthChange(float health, float maxHealth)
    {
        healthSlider.localScale = new Vector3(healthWidth * uiScale.x * (health / maxHealth), healthHeight * uiScale.y, 1);
        text.text = health.ToString("0");
    }

    private void OnDisable()
    {
        target.HealthComponent.OnHealthChanged -= OnHealthChange;
        target.Corruption.OnCorrupting -= OnStabilityChange;
    }
}
