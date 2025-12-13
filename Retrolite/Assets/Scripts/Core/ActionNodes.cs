using UnityEngine;
using System;
using System.Collections;
using Creatures;

namespace CalculatingSystem
{
    [Serializable]
    public abstract class ActionNode
    {
        public abstract void Execute(FormulaContext context);
        public abstract string ToReadableString();
    }

    [Serializable]
    public class DamageAction : ActionNode
    {
        [SerializeReference] public FormulaNode Damage;

        public bool TargetSelf;

        public override void Execute(FormulaContext context)
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

        public override void Execute(FormulaContext context)
        {
            context.Target.HealthComponent.Heal(Amount.Evaluate(context));
        }

        public override string ToReadableString() => $"Heal player for {Amount.ToReadableString()}";
    }

    [Serializable]
    public class GiveResource : ActionNode
    {
        [SerializeReference] public FormulaNode Money;
        public ResourceType resource;

        public override void Execute(FormulaContext context)
        {
            PlayerController.Player.Resources.Get(resource).Add((int)Money.Evaluate(context));
        }

        public override string ToReadableString() => $"Give {Money.ToReadableString()} money";
    }

    [Serializable]
    public class SpawnObjectAction : ActionNode
    {
        public GameObject Prefab;
        public bool onEnemy = false;

        public override void Execute(FormulaContext context)
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

        public override void Execute(FormulaContext context)
        {
            UnityEngine.Object.Destroy(Object);
        }

        public override string ToReadableString() => $"Destroy {Object.name}";
    }

    [Serializable]
    public class AnimationAction : ActionNode
    {
        public string Trigger;

        public override void Execute(FormulaContext context)
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

        public override void Execute(FormulaContext context)
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

        public override void Execute(FormulaContext context)
        {
            context.Owner.StartCoroutine(DelayedExecute(context));
        }

        private IEnumerator DelayedExecute(FormulaContext context)
        {
            yield return new WaitForSeconds(Delay.Evaluate(context));
            Action.Execute(context);
        }

        public override string ToReadableString() => $"Wait for {Delay.ToReadableString()} seconds, then {Action.ToReadableString()}";
    }

    [Serializable]
    public class ExplosionAction : ActionNode
    {
        [SerializeReference] public FormulaNode Damage;
        [SerializeReference] public FormulaNode Knockback;
        [SerializeReference] public FormulaNode Radius;
        public LayerMask Layers;
        public Alignment alignment;

        public override void Execute(FormulaContext context)
        {
            Vector2 position = context.TargetHealth.transform.position;
            ParticleManager.PlayParticle("SmallExplosion", position);
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
