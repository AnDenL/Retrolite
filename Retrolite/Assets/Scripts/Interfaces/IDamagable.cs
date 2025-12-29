using CalculatingSystem;

public interface IDamagable : IKnockbackable
{
    public void Heal(float value);
    public void TakeDamage(float damage);
    public void TakeDamage(float damage, Context context);
}