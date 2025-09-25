using UnityEngine;
using System.Linq;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "PlayerController", menuName = "CreatureAI/Controllers/PlayerController")]
    public class PlayerController : AIController
    {
        private PositionSkill baseMovementSkill;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            baseMovementSkill = owner.ActiveSkills
                .FirstOrDefault(s => s.Type == SkillType.Movement) as PositionSkill;
        }

        public override void UpdateAI()
        {
            Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (baseMovementSkill != null)
            {
                baseMovementSkill.Use(owner.transform.position + (Vector3)moveDir);
            }
        }
    }
}
