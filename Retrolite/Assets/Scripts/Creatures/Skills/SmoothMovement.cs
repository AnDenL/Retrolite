using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "SmoothMovement", menuName = "CreatureAI/Skills/SmoothMovement")]
    public class SmoothMovement : DirectionSkill
    {
        public float Speed = 3;
        public override SkillType Type => SkillType.Movement;

        private int isWalkingHash;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            isWalkingHash = Animator.StringToHash("IsWalking");
        }

        public override bool CanUse(Vector2 direction) => true;

        public override void Activate(Vector2 direction)
        {
            owner.Animator.SetBool(isWalkingHash, direction != Vector2.zero);
            owner.Rb.velocity += owner.Speed * Speed * Time.deltaTime * direction;
            owner.UpdateAnimationState();
        }
    }
}