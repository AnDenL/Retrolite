using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "Dash", menuName = "CreatureAI/Skills/Dash")]
    public class Dash : DirectionSkill
    {
        public float Speed = 3f;
        public float Threshold = 0.25f;

        public override SkillType Type => SkillType.Movement;

        public override void Init(Creature owner)
        {
            base.Init(owner);
        }

        public override void Activate(Vector2 direction)
        {
            owner.Cast();
            owner.Rb.AddForce(Speed * direction, ForceMode2D.Impulse);
        }

        public override bool CanUse(Vector2 position)
        {
            return base.CanUse(position) && owner.Rb.velocity.magnitude < Threshold;
        }
    }
}