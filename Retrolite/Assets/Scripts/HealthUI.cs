using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMesh text;
    [SerializeField] private Transform healthSlider;
    [SerializeField] private SpriteRenderer stabilitySlider;
    [SerializeField] private HealthBase target;

    private Vector2 healthStart;
    private Vector2 stabilityStart;

    private void Start()
    {
        healthStart = healthSlider.transform.localScale;
        stabilityStart = stabilitySlider.transform.localScale;

        target.OnHealthChanged += OnHealthChange;
        target.OnStabilityChange += OnStabilityChange;
        OnHealthChange(target.Health, target.MaxHealth);
        OnStabilityChange(target.Stability);
    }

    private void OnStabilityChange(int stability)
    {
        stabilitySlider.transform.localScale = new Vector3(stability != 0 ? stabilityStart.x / stability : 0, stabilityStart.y, 1);
        stabilitySlider.size = new Vector2(0.2f * stability , 1);
    }

    private void OnDisable()
    {
        target.OnHealthChanged -= OnHealthChange;
        target.OnStabilityChange -= OnStabilityChange;
    }

    private void OnHealthChange(float health, float maxHealth)
    {
        healthSlider.localScale = new Vector3(healthStart.x * target.GetHealthPercent(), healthStart.y, 1);
        text.text = "" + health;
    }
}