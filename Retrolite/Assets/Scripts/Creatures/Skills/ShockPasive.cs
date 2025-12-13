using UnityEngine;
using System.Collections;

namespace Creatures
{
    [CreateAssetMenu(fileName = "ShockPasive", menuName = "CreatureAI/Skills/ShockPasive")]
    public class ShockPasive : PassiveSkill
    {
        public float Damage = 5f;
        public float Radius = 3f;

        public override SkillType Type => SkillType.Attack;

        private Coroutine shockCoroutine;

        public override void Init(Creature owner)
        {
            base.Init(owner);
            owner.HealthComponent.OnDamaged += OnDamaged;
        }

        public void OnDamaged(float current)
        {
            if (owner.HealthComponent.GetHealthPercent() < 0.5f && shockCoroutine == null)
                shockCoroutine = owner.StartCoroutine(ShockCoroutine());
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

            ParticleManager.PlayParticle("Electric", owner.transform.position);
        }
    }
}