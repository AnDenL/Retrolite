using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/Stun")]
public class StunEffect : Effect
{
    public override void OnApply()
    {
        owner.CanAct = false;
        owner.Break();
        ParticleManager.PlayParticle("Stars", owner.transform.position);
    }
    public override void OnRemove() => owner.CanAct = true;
}
