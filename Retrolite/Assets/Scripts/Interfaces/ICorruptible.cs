public interface ICorruptible : IKnockbackable
{
    public void ApplyCorruption(int amount, Creature source);
    public bool Break();
    public void Redact();
}