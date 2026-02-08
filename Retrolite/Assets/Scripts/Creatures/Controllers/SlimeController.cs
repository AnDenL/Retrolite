using UnityEngine;
using System.Linq;

namespace Creatures
{
    [CreateAssetMenu(fileName = "SlimeController", menuName = "CreatureAI/Controllers/SlimeController")]
    public class SlimeController : AIController
    {
        private float checkInterval;
        private Vector2 targetPosition;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            checkInterval = Random.Range(0, 0.5f);
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
                checkInterval = Time.time + 0.5f;

                targetPosition = GetDirectionToTarget();
                Skill chosen = owner.ActiveSkills
                    .OrderByDescending(s => s.Priority)
                    .FirstOrDefault(s => s.CanUse(target));

                if (chosen != null)
                {
                    switch (chosen)
                    {
                        case TargetedSkill targeted:
                            targeted.Use(target);
                            break;
                        case PositionSkill pos:
                            pos.Use(target.transform.position);
                            break;
                        case DirectionSkill dir:
                            dir.Use(targetPosition);
                            break;
                        case SelfSkill self:
                            self.Use();
                            break;
                    }
                }
            }
            else
            {
                if (Movement != null && targetPosition.magnitude != 0)
                {
                    if (Movement.Use(targetPosition))
                    {
                        target = owner.FindTarget();
                    }
                }
            }
        }
    }
}
