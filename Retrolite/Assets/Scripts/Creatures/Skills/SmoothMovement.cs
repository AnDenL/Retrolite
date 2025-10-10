using UnityEngine;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "SmoothMovement", menuName = "CreatureAI/Skills/SmoothMovement")]
    public class SmoothMovement : DirectionSkill
    {
        public float Speed = 3f;

        public override SkillType Type => SkillType.Movement;

        public override void Init(Creature owner)
        {
            base.Init(owner);
        }

        public override bool CanUse(Vector2 direction) => true;

        public override void Activate(Vector2 direction)
        {
            owner.Rb.velocity += direction * Speed * Time.deltaTime;
        }
    }
}