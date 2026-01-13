using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

namespace Creatures
{
    [CreateAssetMenu(fileName = "JumpAttack", menuName = "CreatureAI/Skills/JumpAttack")]
    public class JumpAttack : DirectionSkill
    {
        public float Speed = 3f;
        public float JumpTime = 0.5f;

        public AudioClip jumpSound;
        public AudioClip groundSound;

        private ArcAnim anim;

        public override SkillType Type => SkillType.Movement;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            anim = owner.AddComponent<ArcAnim>();
            anim.duration = JumpTime;
        }

        public override void Activate(Vector2 position)
        {
            owner.Cast();
            owner.PlaySound(jumpSound);
            owner.Animator.SetTrigger("Jump");
            anim.DropTo(owner.transform.position + owner.Speed * Speed * (Vector3)position, () => owner.PlaySound(groundSound));
        }
    }
}