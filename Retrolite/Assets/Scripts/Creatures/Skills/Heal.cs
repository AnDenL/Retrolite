using UnityEngine;
using CalculatingSystem;

namespace Creatures
{
    [CreateAssetMenu(menuName = "CreatureAI/Skills/HealAlly")]
    public class HealAllySkill : AllyTargetedSkill
    {
        public FormulaNode healAmount;

        public override SkillType Type => SkillType.Utility;

        public override void Activate(Creature target)
        {
            if (target == null) return;

            float amount = healAmount.Evaluate(new FormulaContext { Owner = owner, Target = target, TargetHealth = target.HealthComponent });
            target.HealthComponent.Heal(amount);

            ParticleManager.PlayParticle("Heal", target.transform.position);
        }
    }
}