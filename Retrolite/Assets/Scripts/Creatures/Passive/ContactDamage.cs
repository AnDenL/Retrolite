using UnityEngine;

namespace Creatures
{
    [CreateAssetMenu(fileName = "ContactDamage", menuName = "CreatureAI/Skills/ContactDamage")]
    public class ContactDamage : PassiveSkill
    {
        public float damage = 5f;
        public float cooldown = 1f;
        private float lastTime;

        public override SkillType Type => SkillType.Attack;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            owner.CollisionStay2D += OnCollisionEnterEvent;
        }

        public void OnCollisionEnterEvent(Collision2D coll)
        {
            if (Time.time < lastTime || owner.Corruption.IsCorrupted) return;
            if (coll.gameObject.TryGetComponent(out Creature creature))
            {
                if (creature.IsEnemyTo(owner))
                {
                    creature.HealthComponent.TakeDamage(damage);
                    lastTime = Time.time + cooldown;
                }
            }
        }
    }
}