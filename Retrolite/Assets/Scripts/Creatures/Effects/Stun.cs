using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Stun")]
public class StunEffect : Effect
{
    public override void OnApply()
    {
        owner.CanAct = false;
        ParticleManager.PlayParticle("Stars", owner.transform.position);
    }
    public override void OnRemove() => owner.CanAct = true;
}
