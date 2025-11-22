using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "Movement", menuName = "CreatureAI/Skills/Movement")]
    public class Movement : DirectionSkill
    {
        public float Speed = 3f;

        public override SkillType Type => SkillType.Movement;

        public override void Init(Creature owner)
        {
            base.Init(owner);
        }

        public override bool CanUse(Vector2 position) => true;

        public override void Activate(Vector2 position)
        {
            owner.transform.position += Owner.Speed * Speed * Time.deltaTime * (Vector3)position;
        }
    }
}