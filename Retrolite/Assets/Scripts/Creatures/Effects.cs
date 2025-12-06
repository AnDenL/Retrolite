using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public string EffectName;
    public float Strenght;
    public float Duration;

    protected Creature owner;
    protected float timeLeft;

    public virtual void Init(Creature owner)
    {
        this.owner = owner;
        timeLeft = Duration;
        OnApply();
    }

    public virtual void OnApply() { }
    public virtual void OnRemove() { }

    public virtual void Tick(float dt)
    {
        timeLeft -= dt;
    }

    public bool IsFinished => timeLeft <= 0;
}
