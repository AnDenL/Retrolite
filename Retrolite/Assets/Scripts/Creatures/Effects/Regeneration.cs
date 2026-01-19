using UnityEngine;

[CreateAssetMenu(fileName = "Regeneration", menuName = "Game/Effects/Regeneration")]
public class Regeneration : Effect
{
    public override void OnApply()
    {
        
    }

    public override void Tick(float dt)
    {
        owner.HealthComponent.HealthEditable += Strenght * dt;
    }

    public override void OnRemove()
    {
        
    }
}