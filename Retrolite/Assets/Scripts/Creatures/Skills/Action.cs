using UnityEngine;
using CalculatingSystem;

namespace Creatures
{
    [CreateAssetMenu(fileName = "Action", menuName = "CreatureAI/Skills/Action")]
    public class ActionSkill : TargetedSkill
    {
        [SerializeReference] public GameAction action;

        public override SkillType Type => SkillType.Attack;

        public override void Init(Creature owner)
        {
            base.Init(owner);
        }

        public override void Activate(Creature target)
        {
            owner.Cast();
            action.Execute(new Context
            {
                Owner = owner,
                Target = target,
            });
        }
    }
}