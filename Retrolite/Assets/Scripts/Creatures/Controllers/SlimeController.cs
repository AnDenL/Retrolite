using UnityEngine;
using System.Linq;

namespace Creatures
{
    [CreateAssetMenu(fileName = "SlimeController", menuName = "CreatureAI/Controllers/SlimeController")]
    public class SlimeController : AIController
    {
        private float checkInterval = 2f;
        private Vector2 targetPosition;

        public override void Init(Creature owner)
        {
            base.Init(owner);
        }

        public override Vector3 GetDirectionToTarget()
        {
            if (target == null) return Random.insideUnitCircle;
            return (target.transform.position - owner.transform.position).normalized;
        }

        public override void UpdateAI()
        {
            if (checkInterval < Time.time)
            {
                target = owner.FindTarget();
                checkInterval = Time.time + 0.25f;

                targetPosition = GetDirectionToTarget();
                Skill chosen = owner.ActiveSkills
                    .OrderByDescending(s => s.Priority)
                    .FirstOrDefault(s => s.CanUse(target));

                if (chosen != null)
                {
                    if (chosen is TargetedSkill targeted && target != null)
                        targeted.Use(target);
                    else if (chosen is PositionSkill pos)
                        pos.Use(target.transform.position);
                    else if (chosen is DirectionSkill dir)
                        dir.Use(targetPosition);
                    else if (chosen is SelfSkill self)
                        self.Use();
                }
            }
            else
            {
                if (movement != null && targetPosition.magnitude != 0)
                {
                    if (movement.Use(GetDirectionToTarget()))
                    {
                        target = owner.FindTarget();
                    }
                }
            }
        }
    }
}
