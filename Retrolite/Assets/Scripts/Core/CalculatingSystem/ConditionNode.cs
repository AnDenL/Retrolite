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
        public abstract Func<Context, bool> Bake();
    }

    public enum ComparisonOperator { Greater, Less, Equal, NotEqual, GreaterOrEqual, LessOrEqual }

    [Serializable]
    public sealed class ComparisonNode : ConditionNode
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

        public override Func<Context, bool> Bake()
        {
            var a = Left.Bake();
            var b = Right.Bake();

            return Operator switch
            {
                Greater => context => a(context) > b(context),
                Less => context => a(context) < b(context),
                Equal => context => Mathf.Approximately(a(context), b(context)),
                NotEqual => context => !Mathf.Approximately(a(context), b(context)),
                GreaterOrEqual => context => a(context) >= b(context),
                LessOrEqual => context => a(context) <= b(context),
                _ => context => false
            };
        }

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
    public sealed class LogicNode : ConditionNode
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

        public override bool Evaluate(Context context) => Operator switch
        {
            And => Left.Evaluate(context) && Right.Evaluate(context),
            Or => Left.Evaluate(context) || Right.Evaluate(context),
            Not => !Left.Evaluate(context),
            _ => false
        };

        public override string ToReadableString() => Operator switch
        {
            And => $"({Left.ToReadableString()} and {Right.ToReadableString()})",
            Or => $"({Left.ToReadableString()} or {Right.ToReadableString()})",
            Not => $"(not {Left.ToReadableString()})",
            _ => "?"
        };

        public override Func<Context, bool> Bake()
        {
            var a = Left.Bake();
            var b = Right.Bake();
            return Operator switch
            {
                And => context => a(context) && b(context),
                Or => context => a(context) || b(context),
                Not => context => !a(context),
                _ => context => false
            };
        }
    }

    [Serializable]
    public sealed class ConditionVariableNode : ConditionNode
    {
        public ConditionVariable Variable;

        public ConditionVariableNode() { }
        public ConditionVariableNode(ConditionVariable var) => Variable = var;
        public override bool Evaluate(Context context) => ConditionResolver.Resolve(Variable, context);
        public override string ToReadableString() => Variable.ToString();
        public override Func<Context, bool> Bake() => context => ConditionResolver.Resolve(Variable, context);
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

    [Serializable]
    public struct Condition
    {
        [SerializeReference] private ConditionNode rootNode;
        private Func<Context, bool> _cachedFunc;

        public Condition(ConditionNode node)
        {
            rootNode = node;
            _cachedFunc = rootNode.Bake();
        }

        public bool Evaluate(Context context)
        {
            _cachedFunc ??= rootNode.Bake();
            return _cachedFunc(context);
        } 
        
        public readonly string ToReadableString() => rootNode.ToReadableString();
    }

    public enum ConditionVariable
    {
        IsDead,
        IsCorrupted,
        IsBoss,
        IsFullHealth,
    }
}