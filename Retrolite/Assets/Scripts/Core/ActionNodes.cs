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
        public abstract Action<Context> Bake();
    }

    [Serializable]
    public class DamageAction : ActionNode
    {
        public Formula Damage;

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

        public override string ToReadableString() => $"Deal {Damage.ToReadableString()} damage to target";

        public override Action<Context> Bake() => context =>
        {
            Creature creature = TargetSelf ? context.Owner : context.Target;
            creature.HealthComponent.TakeDamage(Damage.Evaluate(context));
            ParticleManager.PlayParticle("Crit", context.TargetHealth.transform.position);
        };
    }

    [Serializable]
    public class HealAction : ActionNode
    {
        public Formula Amount;
        public Formula AdditionalHealth;

        public override void Execute(Context context)
        {
            context.Target.HealthComponent.AddMaximumHealth(AdditionalHealth.Evaluate(context));
            context.Target.HealthComponent.Heal(Amount.Evaluate(context));
            ParticleManager.PlayParticle("Heal", context.Target.transform.position);
        }

        public override string ToReadableString() => $"Heals {Amount.ToReadableString()} hp" + 
        (AdditionalHealth.ToReadableString() == "0" ? "" : $", increases max hp by {AdditionalHealth.ToReadableString()}");

        public override Action<Context> Bake() => context =>
        {
            context.Target.HealthComponent.AddMaximumHealth(AdditionalHealth.Evaluate(context));
            context.Target.HealthComponent.Heal(Amount.Evaluate(context));
            ParticleManager.PlayParticle("Heal", context.Target.transform.position);
        };
    }

    [Serializable]
    public class GiveResource : ActionNode
    {
        public Formula Count;
        public ResourceType resource;

        public override void Execute(Context context)
        {
            int money = (int)Count.Evaluate(context);
            context.Target.Resources.Get(resource).Add(money);
            ParticleManager.PlayParticle(resource, context.Owner.transform.position, context.Target.transform, money);
        }

        public override string ToReadableString() => $"Give {Count.ToReadableString()} {resource}";
        public override Action<Context> Bake() => context =>
        {
            int money = (int)Count.Evaluate(context);
            context.Target.Resources.Get(resource).Add(money);
            ParticleManager.PlayParticle(resource, context.Owner.transform.position, context.Target.transform, money);
        };
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

        public override Action<Context> Bake() => context =>
            UnityEngine.Object.Instantiate(Prefab, context.Position, Quaternion.identity);
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
        public override Action<Context> Bake() => context => UnityEngine.Object.Destroy(Object);
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

        public override Action<Context> Bake() => context => context.Owner.Animator.SetTrigger(Trigger);
    }

    [Serializable]
    public class PlayParticleAction : ActionNode
    {
        public string Particles;

        public override void Execute(Context context)
        {
            ParticleManager.PlayParticle(Particles, context.Position);
        }

        public override string ToReadableString() => $"Play particle {Particles}";
        public override Action<Context> Bake() => context => ParticleManager.PlayParticle(Particles, context.Position);
    }

    [Serializable]
    public class DelayedAction : ActionNode
    {
        public Formula Delay;
        public ActionNode Action;

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

        public override Action<Context> Bake() => context => context.Owner.StartCoroutine(DelayedExecute(context));
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

        public override Action<Context> Bake() => context => context.Target.AddEffect(effect, strength.Evaluate(context), duration.Evaluate(context));
    }

    [Serializable]
    public class ExplosionAction : ActionNode
    {
        public Formula Damage;
        public Formula Knockback;
        public Formula Radius;
        public LayerMask Layers;
        public string Particle = "SmallExplosion";

        public override void Execute(Context context)
        {
            Vector2 position = context.Position;
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

        public override Action<Context> Bake() => context =>
        {
            Vector2 position = context.Position;
            ParticleManager.PlayParticle(Particle, position);
            var hits = Physics2D.OverlapCircleAll(position, Radius.Evaluate(context), Layers);

            float damage = Damage.Evaluate(context); 

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Creature creature))
                {
                    if (!creature.IsEnemyTo(context.Owner)) continue;
                    creature.HealthComponent.TakeDamage(damage);
                    Vector2 dir = hit.transform.position - (Vector3)position;
                    creature.Rb.AddForce(Knockback.Evaluate(context) * dir, ForceMode2D.Impulse);
                }
            }  
        };
    }

    [Serializable]
    public struct GameAction
    {
        [SerializeReference] public ActionNode rootNode;
        private Action<Context> _cachedFunc;

        public GameAction(ActionNode node)
        {
            rootNode = node;
            _cachedFunc = rootNode.Bake();
        }

        public void Execute(Context context)
        {
            _cachedFunc ??= rootNode.Bake();
            _cachedFunc(context);
        } 

        public readonly string ToReadableString() => rootNode.ToReadableString();
    }
}
