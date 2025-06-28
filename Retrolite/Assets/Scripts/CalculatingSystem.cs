namespace CalculatingSystem
{
    using System;
    using UnityEngine;
    using static CalculatingSystem.Operator;
    using static CalculatingSystem.StatVariable;

    [Serializable]
    public abstract class FormulaNode
    {
        public abstract float Evaluate(FormulaContext context);
        public abstract string ToReadableString();
        public abstract bool IsConstant();
    }

    [Serializable]
    public class ConstantNode : FormulaNode
    {
        public float Value;
        public override bool IsConstant() => true;

        public ConstantNode() => Value = 0;

        public ConstantNode(float value) => Value = value;
        public override float Evaluate(FormulaContext context) => Value;
        public override string ToReadableString() => Value.ToString();
    }

    [Serializable]
    public class SinNode : FormulaNode
    {
        [SerializeReference]
        public FormulaNode Node;
        public override bool IsConstant() => Node.IsConstant();

        public SinNode() {}

        public SinNode(FormulaNode value) => Node = value;
        public override float Evaluate(FormulaContext context) => Mathf.Sin(Node.Evaluate(context));
        public override string ToReadableString() => "Sin(" + Node.ToString() + ")";
    }

    [Serializable]
    public class CosNode : FormulaNode
    {
        [SerializeReference]
        public FormulaNode Node;
        public override bool IsConstant() => Node.IsConstant();

        public CosNode() {}

        public CosNode(FormulaNode value) => Node = value;
        public override float Evaluate(FormulaContext context) => Mathf.Cos(Node.Evaluate(context));
        public override string ToReadableString() => "Cos(" + Node.ToString() + ")";
    }

    [Serializable]
    public class VariableNode : FormulaNode
    {
        public StatVariable Variable;
        public override bool IsConstant() => false;

        public VariableNode() { }
        public VariableNode(StatVariable var) => Variable = var;
        public override float Evaluate(FormulaContext context) => VariableResolver.Resolve(Variable, context);
        public override string ToReadableString() => Variable.ToString();
    }

    [Serializable]
    public class Expression : FormulaNode
    {
        [SerializeReference]
        public FormulaNode Left;
        public Operator Operation;
        [SerializeReference]
        public FormulaNode Right;
        public override bool IsConstant() => Left.IsConstant() && Right.IsConstant();

        public Expression() { }

        public Expression(FormulaNode left, Operator op, FormulaNode right)
        {
            Left = left;
            Right = right;
            Operation = op;
        }

        public override float Evaluate(FormulaContext context)
        {
            float a = Left.Evaluate(context);
            float b = Right.Evaluate(context);
            return Operation switch
            {
                Add => a + b,
                Subtract => a - b,
                Multiply => a * b,
                Divide => b == 0 ? 0 : a / b,
                _ => 0
            };
        }

        public override string ToReadableString() =>
            $"({Left.ToReadableString()} {OpToString(Operation)} {Right.ToReadableString()})";

        public string OpToString(Operator op) => op switch
        {
            Add => "+",
            Subtract => "-",
            Multiply => "*",
            Divide => "/",
            _ => "?"
        };
    }

    public enum Operator { Add, Subtract, Multiply, Divide }

    public struct FormulaContext
    {
        public HealthBase Health;
        public GunBase Gun;
        public BulletBase Bullet;
    }

    public static class VariableResolver
    {
        public static float Resolve(StatVariable variable, FormulaContext context)
        {
            return variable switch
            {
                PlayerHP => Player.instance.GetHealthPercent(),
                EnemyHP => context.Health?.GetHealthPercent() ?? Break(variable, context),
                BulletTime => context.Bullet?.GetLifetime() ?? Break(variable, context),
                Echo => context.Gun.Data.Echo,
                Distance => context.Bullet?.GetDistanceTravelled() ?? Break(variable, context),
                PlayerDistance => Vector2.Distance(Player.instance.transform.position, context.Bullet?.transform.position ?? Vector3.zero),
                Ammo => context.Gun?.Data.CurrentAmmo ?? Break(variable, context),
                RandomNum => UnityEngine.Random.Range(-5f, 5f),
                Money => Player.instance.GetMoney(),
                Speed => context.Bullet?.speed ?? Break(variable, context),
                Size => context.Bullet?.scale ?? Break(variable, context),
                BulletSpread => context.Bullet?.spread ?? Break(variable, context) * Mathf.Deg2Rad,
                BulletDestroyTime => context.Bullet?.GetDestroyTime() ?? Break(variable, context),
                _ => 0f
            };
        }
        public static float Break(StatVariable variable, FormulaContext context)
        {
            return 0;
        }
    }

    public enum StatVariable
    {
        PlayerHP,
        EnemyHP,
        BulletTime,
        Echo,
        Distance,
        PlayerDistance,
        Ammo,
        RandomNum,
        Money,
        Speed,
        Size,
        BulletSpread,
        BulletDestroyTime
    }
}
