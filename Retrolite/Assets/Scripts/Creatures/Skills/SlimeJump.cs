using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "JumpAttack", menuName = "CreatureAI/Skills/JumpAttack")]
    public class JumpAttack : DirectionSkill
    {
        public float Speed = 3f;
        public float JumpTime = 0.5f;

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
            anim.DropTo(owner.transform.position + (Vector3)position * Speed);
        }
    }
}