using UnityEngine;
using System.Linq;

namespace CreatureAI
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

        public override void UpdateAI()
        {
            if (checkInterval < Time.time)
            {
                target = owner.FindTarget();
                checkInterval = Time.time + 1f;


                if (target != null)
                {
                    targetPosition = target.transform.position - owner.transform.position;
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
                    targetPosition = Random.insideUnitCircle;
            }
            else
            {
                if (movement != null && targetPosition.magnitude != 0)
                {
                    if (movement.Use(targetPosition))
                    {
                        target = owner.FindTarget();
                    }
                }
            }
        }
    }
}
