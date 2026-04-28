using UnityEngine;
using System;
using System.Collections;
using Creatures;

namespace CalculatingSystem
{
    [Serializable]
    public abstract class ActionNode
    {
        public abstract void Execute(Context context);
        public abstract string ToReadableString();
    }

    [Serializable]
    public class DamageAction : ActionNode
    {
        public Formula Damage;

        public bool TargetSelf;

        public override void Execute(Context context)
        {
            if (context.Target != null)
            {
                Creature creature = TargetSelf ? context.Owner : context.Target;
                float damage = Damage.Evaluate(context);
                creature.HealthComponent.TakeDamage(damage);
                ParticleManager.PlayParticle("Crit", context.Target.HealthComponent.transform.position, (int)damage);
            }
        }

        public override string ToReadableString() => $"Deal {Damage.ToReadableString()} damage to target";
    }

    [Serializable]
    public class HealAction : ActionNode
    {
        public Formula Amount;
        public Formula AdditionalHealth;

        public override void Execute(Context context)
        {
            float heal = Amount.Evaluate(context);
            context.Target.HealthComponent.AddMaximumHealth(AdditionalHealth.Evaluate(context));
            context.Target.HealthComponent.Heal(heal);
            ParticleManager.PlayParticle("Heal", context.Target.transform.position, (int)heal);
        }

        public override string ToReadableString() => $"Heals {Amount.ToReadableString()} hp" + 
        (AdditionalHealth.ToReadableString() == "0" ? "" : $", increases max hp by {AdditionalHealth.ToReadableString()}");
    }

    [Serializable]
    public class GiveResource : ActionNode
    {
        public Formula Amount;
        public ResourceType Resource;

        public override void Execute(Context context)
        {
            int money = (int)Amount.Evaluate(context);
            context.Target.Resources.Get(Resource).Add(money);
            ParticleManager.PlayParticle(Resource, context.Owner.transform.position, context.Target.transform, money);
        }

        public override string ToReadableString() => $"Give {Amount.ToReadableString()} {Resource}";
    }

    [Serializable]
    public class SpawnObjectAction : ActionNode
    {
        public GameObject Prefab;

        public override void Execute(Context context)
        {
            if (Prefab != null && context.Position != null)
                UnityEngine.Object.Instantiate(Prefab, context.Position, Quaternion.identity);
        }

        public override string ToReadableString() => $"Spawn {Prefab.name}";
    }

    [Serializable]
    public class DestroyObjectAction : ActionNode
    {
        public GameObject Object;

        public override void Execute(Context context)
        {
            UnityEngine.Object.Destroy(Object);
        }

        public override string ToReadableString() => $"Destroy {Object.name}";
    }

    [Serializable]
    public class AnimationAction : ActionNode
    {
        public string Trigger;

        public override void Execute(Context context)
        {
            if (context.Owner.Animator != null)
                context.Owner.Animator.SetTrigger(Trigger);
        }

        public override string ToReadableString() => $"Play animation {Trigger}";
    }

    [Serializable]
    public class PlayParticleAction : ActionNode
    {
        public string Particles;
        public int Count = 5;

        public override void Execute(Context context)
        {
            ParticleManager.PlayParticle(Particles, context.Position, Count);
        }

        public override string ToReadableString() => $"Play particle {Particles}";
    }

    [Serializable]
    public class DelayedAction : ActionNode
    {
        public Formula Delay;
        [SerializeReference] public ActionNode Action;

        public override void Execute(Context context)
        {
            context.Owner.StartCoroutine(DelayedExecute(context));
        }

        private IEnumerator DelayedExecute(Context context)
        {
            yield return new WaitForSeconds(Delay.Evaluate(context));
            Action.Execute(context);
        }

        public override string ToReadableString() => $"Wait for {Delay.ToReadableString()} seconds, then {Action.ToReadableString()}";
    }

    [Serializable]
    public class ApplyEffectAction : ActionNode
    {
        public Formula strength;
        public Formula duration;
        public Effect effect;

        public override void Execute(Context context)
        {
            context.Target.AddEffect(effect, strength.Evaluate(context), duration.Evaluate(context));
        }

        public override string ToReadableString() => $"Apply {effect.EffectName}, for {duration.ToReadableString()} seconds";
    }

    [Serializable]
    public class ExplosionAction : ActionNode
    {
        public Formula Damage;
        public Formula Knockback;
        public Formula Radius;
        public LayerMask Layers;
        public string Particle = "SmallExplosion";
        public int ParticleCount = 15;

        private readonly Collider2D[] cachedColl = new Collider2D[32];

        public override void Execute(Context context)
        {
            Vector2 position = context.Position;
            ParticleManager.PlayParticle(Particle, position, ParticleCount);
            int hits = Physics2D.OverlapCircleNonAlloc(position, Radius.Evaluate(context), cachedColl, Layers);

            for (int i = 0; i < hits; i++)
            {
                if (cachedColl[i].TryGetComponent(out Creature creature))
                {
                    if (!creature.IsEnemyTo(context.Owner)) continue;
                    creature.HealthComponent.TakeDamage(Damage.Evaluate(context));
                    Vector2 dir = cachedColl[i].transform.position - (Vector3)position;
                    creature.Rb.AddForce(Knockback.Evaluate(context) * dir, ForceMode2D.Impulse);
                }
            }
        }

        public override string ToReadableString() => $"Creates explosion(damage:{Damage.ToReadableString()}, knockback:{Knockback.ToReadableString()}, radius:{Radius.ToReadableString()})";
    }
}
