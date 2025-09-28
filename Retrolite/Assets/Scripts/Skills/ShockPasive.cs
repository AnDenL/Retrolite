using UnityEngine;
using System.Collections;

namespace CreatureAI
{
    [CreateAssetMenu(fileName = "ShockPasive", menuName = "CreatureAI/Skills/ShockPasive")]
    public class ShockPasive : PassiveSkill
    {
        public float Damage = 5f;
        public float Radius = 3f;

        public override SkillType Type => SkillType.Attack;

        private Coroutine shockCoroutine;

        public override void Subscribe(Creature owner)
        {
            base.Subscribe(owner);
            owner.HealthComponent.OnHealthChanged += CheckAndStartShock;
        }

        public void CheckAndStartShock(float current, float max)
        {
            if (owner.HealthComponent.GetHealthPercent() < 0.5f && shockCoroutine == null)
                shockCoroutine = owner.StartCoroutine(ShockCoroutine());
            else if (shockCoroutine != null) owner.StopCoroutine(shockCoroutine);
        }

        private IEnumerator ShockCoroutine()
        {
            while (!owner.HealthComponent.IsDead)
            {
                Shock();
                yield return new WaitForSeconds(0.5f + owner.HealthComponent.GetHealthPercent());
            }
        }

        private void Shock()
        {
            var hits = Physics2D.OverlapCircleAll(owner.transform.position, Radius, LayerMask.GetMask("Creatures"));

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Creature creature))
                {
                    if (creature.IsEnemyTo(owner)) creature.HealthComponent.TakeDamage(Damage);
                }
            }

            ParticleManager.PlayParticle(4, owner.transform.position);
        }
    }
}