using CalculatingSystem;
using UnityEngine;

public class ActionOnCorrupt : MonoBehaviour, ICorruptible
{
    public bool IsCorrupted { get; set;}

    [SerializeReference] public ActionNode Action;
    public bool DoBreak;
    public bool SigleTime;
    
    public void ApplyCorruption(int amount, Creature source)
    {
        if (TryGetComponent(out Animator animator)) animator.SetTrigger("Corrupt");
        Action.Execute(new Context { Owner = source, Target = source });
        if (SigleTime) Destroy(this);
    }

    public bool Break()
    {
        return DoBreak;
    }

    public void Knockback(Vector2 dir, float strength) {}
    public void Redact() {}
}
