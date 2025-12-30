using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "TurretController", menuName = "CreatureAI/Controllers/TurretController")]
    public class TurretController : AIController
    {
        private readonly float checkInterval = 0.5f;
        private float lastTime;
        private List<Skill> skills;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            skills = owner.ActiveSkills.Where(s => s.Type == SkillType.Attack).ToList();
        }

        public override void UpdateAI()
        {
            if (Time.time > lastTime + checkInterval)
            {
                target = owner.FindTarget();
                lastTime = Time.time;

                Skill chosen = skills.OrderByDescending(s => s.Priority).FirstOrDefault(s => s.CanUse(target));

                switch (chosen)
                {
                    case TargetedSkill targeted:
                        targeted.Use(target);
                        break;
                    case PositionSkill pos:
                        pos.Use(target.transform.position);
                        break;
                    case DirectionSkill dir:
                        dir.Use(GetDirectionToTarget());
                        break;
                    case SelfSkill self:
                        self.Use();
                        break;
                }
            }
        }
    }
}