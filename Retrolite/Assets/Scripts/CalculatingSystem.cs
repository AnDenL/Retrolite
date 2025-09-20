namespace CalculatingSystem
{
    using System;
    using UnityEngine;
    using static CalculatingSystem.Operator;
    using static CalculatingSystem.StatVariable;
    using static CalculatingSystem.LogicOperator;
    using static CalculatingSystem.ComparisonOperator;
    using static CalculatingSystem.ConditionVariable;

    #region FormulaNodes

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
    public class AbsoluteNode : FormulaNode
    {
        [SerializeReference]
        public FormulaNode Node;
        public override bool IsConstant() => true;

        public AbsoluteNode() => Node = new ConstantNode(0);

        public AbsoluteNode(FormulaNode node) => Node = node;
        public override float Evaluate(FormulaContext context) => Math.Abs(Node.Evaluate(context));
        public override string ToReadableString() => "|" + Node.ToReadableString() + "|";
    }

    [Serializable]
    public class SinNode : FormulaNode
    {
        [SerializeReference]
        public FormulaNode Node;
        public override bool IsConstant() => Node.IsConstant();

        public SinNode() { }

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

        public CosNode() { }

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
    #endregion

    public enum Operator { Add, Subtract, Multiply, Divide }

    #region Variables

    public static class VariableResolver
    {
        public static float Resolve(StatVariable variable, FormulaContext context)
        {
            return variable switch
            {
                PlayerHP => Player.instance.GetHealthPercent(),
                EnemyHP => context.TargetHealth?.GetHealthPercent() ?? Break(variable, context),
                BulletTime => context.Bullet?.GetLifetime() ?? Break(variable, context),
                Echo => context.Gun.Data.Echo,
                Distance => context.Bullet?.GetDistanceTravelled() ?? Break(variable, context),
                PlayerDistance => Vector2.Distance(Player.instance.transform.position, context.Bullet?.transform.position ?? Vector3.zero),
                Ammo => context.Gun?.Data.CurrentAmmo ?? Break(variable, context),
                RandomNum => UnityEngine.Random.Range(-5f, 5f),
                Money => Player.instance.GetMoney(),
                Speed => context.Bullet?.Speed ?? Break(variable, context),
                Size => context.Bullet?.Scale ?? Break(variable, context),
                BulletSpread => context.Bullet?.Spread ?? Break(variable, context) * Mathf.Deg2Rad,
                BulletDestroyTime => context.Bullet?.GetDestroyTime() ?? Break(variable, context),
                HomingAngle => Utilities.CalculateHomingAngle(context),
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
        BulletDestroyTime,
        HomingAngle
    }

    #endregion
    #region ConditionNodes

    [Serializable]
    public abstract class ConditionNode
    {
        public abstract bool Evaluate(FormulaContext context);
        public abstract string ToReadableString();
    }

    public enum ComparisonOperator { Greater, Less, Equal, NotEqual, GreaterOrEqual, LessOrEqual }

    [Serializable]
    public class ComparisonNode : ConditionNode
    {
        [SerializeReference]
        public FormulaNode Left;
        [SerializeReference]
        public FormulaNode Right;
        public ComparisonOperator Operator;

        public ComparisonNode() { }

        public ComparisonNode(FormulaNode left, ComparisonOperator op, FormulaNode right)
        {
            Left = left;
            Right = right;
            Operator = op;
        }

        public override bool Evaluate(FormulaContext context)
        {
            float a = Left.Evaluate(context);
            float b = Right.Evaluate(context);

            return Operator switch
            {
                Greater => a > b,
                Less => a < b,
                Equal => Mathf.Approximately(a, b),
                NotEqual => !Mathf.Approximately(a, b),
                GreaterOrEqual => a >= b,
                LessOrEqual => a <= b,
                _ => false
            };
        }

        public override string ToReadableString() =>
            $"({Left.ToReadableString()} {OperatorToString()} {Right.ToReadableString()})";

        private string OperatorToString() => Operator switch
        {
            Greater => ">",
            Less => "<",
            Equal => "==",
            NotEqual => "!=",
            GreaterOrEqual => ">=",
            LessOrEqual => "<=",
            _ => "?"
        };
    }
    public enum LogicOperator { And, Or, Not }

    [Serializable]
    public class LogicNode : ConditionNode
    {
        [SerializeReference]
        public ConditionNode Left;
        [SerializeReference]
        public ConditionNode Right;
        public LogicOperator Operator;

        public LogicNode() { }

        public LogicNode(ConditionNode left, LogicOperator logicOperator, ConditionNode right)
        {
            Left = left;
            Operator = logicOperator;
            Right = right;
        }

        public override bool Evaluate(FormulaContext context)
        {
            return Operator switch
            {
                And => Left.Evaluate(context) && Right.Evaluate(context),
                Or => Left.Evaluate(context) || Right.Evaluate(context),
                Not => !Left.Evaluate(context),
                _ => false
            };
        }

        public override string ToReadableString() =>
            Operator switch
            {
                And => $"({Left.ToReadableString()} and {Right.ToReadableString()})",
                Or => $"({Left.ToReadableString()} or {Right.ToReadableString()})",
                Not => $"(not {Left.ToReadableString()})",
                _ => "?"
            };
    }

    [Serializable]
    public class ConditionVariableNode : ConditionNode
    {
        public ConditionVariable Variable;

        public ConditionVariableNode() { }
        public ConditionVariableNode(ConditionVariable var) => Variable = var;
        public override bool Evaluate(FormulaContext context) => ConditionResolver.Resolve(Variable, context);
        public override string ToReadableString() => Variable.ToString();
    }

    public static class ConditionResolver
    {
        public static bool Resolve(ConditionVariable variable, FormulaContext context)
        {
            return variable switch
            {
                IsDead => context.TargetHealth.IsDead,
                IsCorrupted => context.TargetCreature.Corruption.Stability == 0,
                IsBoss => false, //Change later
                IsFullHealth => context.TargetHealth.Health == context.TargetHealth.MaxHealth,
                _ => false
            };
        }
    }

    public enum ConditionVariable
    {
        IsDead,
        IsCorrupted,
        IsBoss,
        IsFullHealth,
    }

    #endregion

    public struct FormulaContext
    {
        public HealthBase TargetHealth;
        public Creature TargetCreature;
        public Creature Owner;
        public GunBase Gun;
        public BulletBase Bullet;
    }
}
