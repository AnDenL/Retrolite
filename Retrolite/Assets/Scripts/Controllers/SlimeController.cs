using UnityEngine;
using System.Linq;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "SlimeController", menuName = "CreatureAI/Controllers/SlimeController")]
    public class SlimeController : AIController
    {
        private float checkInterval = 2f;
        private PositionSkill baseMovementSkill;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            baseMovementSkill = owner.ActiveSkills
                .FirstOrDefault(s => s.Type == SkillType.Movement) as PositionSkill;
        }

        public override void UpdateAI()
        {
            if (checkInterval < Time.time)
            {
                target = owner.FindTarget();
                checkInterval = Time.time + 2f;
            }
            if (target != null)
            {
                Skill chosen = owner.ActiveSkills
                    .OrderByDescending(s => s.Priority)
                    .FirstOrDefault(s => s.CanUse(target));

                if (chosen != null)
                {
                    if (chosen is TargetedSkill targeted)
                        targeted.Use(target);
                    else if (chosen is PositionSkill pos)
                        pos.Use(target.transform.position);
                    else if (chosen is SelfSkill self)
                        self.Use();
                }
            }
            else
            {
                if (baseMovementSkill != null)
                {
                    Vector3 randomDir = Random.insideUnitCircle.normalized;
                    if (baseMovementSkill.Use(owner.transform.position + randomDir))
                    {
                        target = owner.FindTarget();
                    }
                }
            }
        }
    }
}
