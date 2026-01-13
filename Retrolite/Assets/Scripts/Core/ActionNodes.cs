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
        [SerializeReference] public FormulaNode Damage;

        public bool TargetSelf;

        public override void Execute(Context context)
        {
            if (context.TargetHealth != null)
            {
                Creature creature = TargetSelf ? context.Owner : context.Target;
                creature.HealthComponent.TakeDamage(Damage.Evaluate(context));
                ParticleManager.PlayParticle("Crit", context.TargetHealth.transform.position);
            }
        }

        public override string ToReadableString() => $"Deal {Damage.ToReadableString()} damage to enemy";
    }

    [Serializable]
    public class HealAction : ActionNode
    {
        [SerializeReference] public FormulaNode Amount;
        [SerializeReference] public FormulaNode AdditionalHealth;

        public override void Execute(Context context)
        {
            context.Target.HealthComponent.AddMaximumHealth(AdditionalHealth.Evaluate(context));
            context.Target.HealthComponent.Heal(Amount.Evaluate(context));
            ParticleManager.PlayParticle("Heal", context.Target.transform.position);
        }

        public override string ToReadableString() => $"Heal player for {Amount.ToReadableString()}" + 
        (AdditionalHealth.ToReadableString() == "0" ? "" : $", increases maximum health by {AdditionalHealth.ToReadableString()}");
    }

    [Serializable]
    public class GiveResource : ActionNode
    {
        [SerializeReference] public FormulaNode Money;
        public ResourceType resource;

        public override void Execute(Context context)
        {
            int money = (int)Money.Evaluate(context);
            context.Target.Resources.Get(resource).Add(money);
            ParticleManager.PlayParticle(resource, context.Owner.transform.position, context.Target.transform, money);
        }

        public override string ToReadableString() => $"Give {Money.ToReadableString()} money";
    }

    [Serializable]
    public class SpawnObjectAction : ActionNode
    {
        public GameObject Prefab;
        public bool onEnemy = false;

        public override void Execute(Context context)
        {
            if (Prefab != null && context.TargetHealth != null)
                UnityEngine.Object.Instantiate(Prefab, onEnemy ? context.TargetHealth.transform.position : context.Owner.transform.position, Quaternion.identity);
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

        public override void Execute(Context context)
        {
            if (context.Target.HealthComponent != null)
                ParticleManager.PlayParticle(Particles, context.Owner.transform.position);
        }

        public override string ToReadableString() => $"Play particle {Particles}";
    }

    [Serializable]
    public class DelayedAction : ActionNode
    {
        [SerializeReference] public FormulaNode Delay;
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
        [SerializeReference] public FormulaNode strength;
        [SerializeReference] public FormulaNode duration;
        public Effect effect;

        public override void Execute(Context context)
        {
            context.Target.AddEffect(effect, strength.Evaluate(context), duration.Evaluate(context));
        }

        public override string ToReadableString() => $"";
    }

    [Serializable]
    public class ExplosionAction : ActionNode
    {
        [SerializeReference] public FormulaNode Damage;
        [SerializeReference] public FormulaNode Knockback;
        [SerializeReference] public FormulaNode Radius;
        public LayerMask Layers;
        public string Particle = "SmallExplosion";

        public override void Execute(Context context)
        {
            Vector2 position = context.Target.transform.position;
            ParticleManager.PlayParticle(Particle, position);
            var hits = Physics2D.OverlapCircleAll(position, Radius.Evaluate(context), Layers);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Creature creature))
                {
                    if (!creature.IsEnemyTo(context.Owner)) continue;
                    creature.HealthComponent.TakeDamage(Damage.Evaluate(context));
                    Vector2 dir = hit.transform.position - (Vector3)position;
                    creature.Rb.AddForce(Knockback.Evaluate(context) * dir, ForceMode2D.Impulse);
                }
            }
        }

        public override string ToReadableString() => $"Creates explosion(damage:{Damage.ToReadableString()}, knockback:{Knockback.ToReadableString()}, radius:{Radius.ToReadableString()})";
    }
}
