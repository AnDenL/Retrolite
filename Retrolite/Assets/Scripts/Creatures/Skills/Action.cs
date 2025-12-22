using UnityEngine;
using System.Collections;
using CalculatingSystem;

namespace Creatures
{
    [CreateAssetMenu(fileName = "Action", menuName = "CreatureAI/Skills/Action")]
    public class ActionSkill : TargetedSkill
    {
        [SerializeReference] public ActionNode action;

        public override SkillType Type => SkillType.Attack;

        public override void Init(Creature owner)
        {
            base.Init(owner);
        }

        public override void Activate(Creature target)
        {
            action.Execute(new FormulaContext
            {
                Owner = owner,
                Target = target,
                TargetHealth = target.HealthComponent,
                Point = target.transform
            });
        }
    }
}