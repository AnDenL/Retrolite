namespace CalculatingSystem
{
    using System;
    using UnityEngine;
    using static CalculatingSystem.LogicOperator;
    using static CalculatingSystem.ComparisonOperator;
    using static CalculatingSystem.ConditionVariable;

    [Serializable]
    public abstract class ConditionNode
    {
        public abstract bool Evaluate(Context context);
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

        public override bool Evaluate(Context context)
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

        public override bool Evaluate(Context context)
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
        public override bool Evaluate(Context context) => ConditionResolver.Resolve(Variable, context);
        public override string ToReadableString() => Variable.ToString();
    }

    public static class ConditionResolver
    {
        public static bool Resolve(ConditionVariable variable, Context context)
        {
            return variable switch
            {
                IsDead => context.TargetHealth.IsDead,
                IsCorrupted => context.Target.Corruption.Stability == 0,
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
}