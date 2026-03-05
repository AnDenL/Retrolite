public interface ICorruptible
{
    public bool IsCorrupted { get; set; }
    public void ApplyCorruption(int amount, Creature source);
    public bool Break();
    public void Redact();
}