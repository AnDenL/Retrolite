using UnityEngine;
using System;

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

        public override void Execute(FormulaContext context)
        {
            if (context.EnemyHealth != null)
            {
                context.EnemyHealth.TakeDamage(Damage.Evaluate(context));
                ParticleManager.PlayParticle(3, context.EnemyHealth.transform.position);
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
            Player.instance.Heal(Amount.Evaluate(context));
        }

        public override string ToReadableString() => $"Heal player for {Amount.ToReadableString()}";
    }

    [Serializable]
    public class GiveMoneyAction : ActionNode
    {
        [SerializeReference] public FormulaNode Money;

        public override void Execute(FormulaContext context)
        {
            Player.instance.AddMoney((int)Money.Evaluate(context));
        }

        public override string ToReadableString() => $"Give {Money.ToReadableString()} money";
    }

    [Serializable]
    public class GiveCodeAction : ActionNode
    {
        [SerializeReference] public FormulaNode Code;

        public override void Execute(FormulaContext context)
        {
            Player.instance.AddCode((int)Code.Evaluate(context));
        }

        public override string ToReadableString() => $"Give {Code.ToReadableString()} money";
    }

    [Serializable]
    public class SpawnObjectAction : ActionNode
    {
        public GameObject Prefab;

        public override void Execute(FormulaContext context)
        {
            if (Prefab != null && context.EnemyHealth != null)
                UnityEngine.Object.Instantiate(Prefab, context.EnemyHealth.transform.position, Quaternion.identity);
        }

        public override string ToReadableString() => $"Spawn {Prefab?.name}";
    }

    [Serializable]
    public class DestroyObjectAction : ActionNode
    {
        public GameObject Object;

        public override void Execute(FormulaContext context)
        {
            UnityEngine.Object.Destroy(Object);
        }

        public override string ToReadableString() => $"Destroy {Object?.name}";
    }

    [Serializable]
    public class ExplosionAction : ActionNode
    {
        [SerializeReference] public FormulaNode Damage;
        [SerializeReference] public FormulaNode Knockback;
        [SerializeReference] public FormulaNode Radius;
        public LayerMask Layers;

        public override void Execute(FormulaContext context)
        {
            Vector2 position = context.EnemyHealth.transform.position;
            ParticleManager.PlayParticle(2, position);
            var hits = Physics2D.OverlapCircleAll(position, Radius.Evaluate(context), Layers);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out HealthBase health))
                {
                    health.TakeDamage(Damage.Evaluate(context));
                    health.Knockback?.StartKnockback(Knockback.Evaluate(context), hit.transform.position - (Vector3)position);
                }
            }
        }

        public override string ToReadableString() => $"Creates explosion(damage:{Damage.ToReadableString()}, knockback:{Knockback.ToReadableString()}, radius:{Radius.ToReadableString()})";
    }
}