using UnityEngine;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "Movement", menuName = "CreatureAI/Skills/Movement")]
    public class Movement : PositionSkill
    {
        public float Speed = 3f;

        public override SkillType Type => SkillType.Movement;

        public override void Init(Creature owner)
        {
            base.Init(owner);
        }

        public override void Activate(Vector3 position)
        {
            owner.transform.position += Speed * Time.deltaTime * (position - owner.transform.position);
        }
    }
}