using UnityEngine;
using System.Linq;

namespace Creatures
{
    [CreateAssetMenu(fileName = "RobotController", menuName = "CreatureAI/Controllers/RobotController")]
    public class RobotController : AIController
    {
        private float checkTime;
        private bool isSleeping = true;

        public override void Init(Creature owner)
        {
            base.Init(owner);

            if (owner.HealthComponent != null)
                owner.HealthComponent.OnDamaged += (_) => WakeUp();
        }

        private void WakeUp()
        {
            if (!isSleeping) return;
            isSleeping = false;
            if (owner.Animator != null)
                owner.Animator.SetBool("IsSleeping", false);
        }

        private void GoToSleep()
        {
            isSleeping = true;
            if (owner.Animator != null)
                owner.Animator.SetBool("IsSleeping", true);
        }

        public override void UpdateAI()
        {
            if (owner.HealthComponent.IsDead || owner.Corruption.IsCorrupted) return;

            if (Time.time > checkTime)
            {
                target = owner.FindTarget();
                checkTime = Time.time + 1f;
            }

            if (isSleeping && target == null)
            {
                if (owner.Animator != null)
                    owner.Animator.SetBool("IsSleeping", true);
                return;
            }

            if (target != null) WakeUp();

            if (target != null)
            {
                float dist = Vector2.Distance(owner.transform.position, target.transform.position);
                owner.LookAt(target.transform.position);

                foreach (var skill in owner.ActiveSkills.OfType<TargetedSkill>())
                {
                    if (skill.CanUse(target))
                        skill.Use(target);
                }
                if (dist < 3f)
                {
                    foreach (var escape in owner.ActiveSkills
                        .Where(s => s.Type == SkillType.Movement || s.Type == SkillType.Defense))
                    {
                        Vector2 dir = (owner.transform.position - target.transform.position).normalized;
                        escape.Use(owner.transform.position + (Vector3)dir);
                    }

                    movement.Use(-GetDirectionToTarget());
                }
                else if (dist > 7f)
                {
                    movement.Use(GetDirectionToTarget());
                }
            }
            else
            {
                if (!isSleeping)
                {
                    GoToSleep();
                }
            }
        }
    }
}
