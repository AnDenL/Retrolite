namespace CalculatingSystem
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;
    using static ConditionVariable;

    [Serializable]
    public abstract class ConditionNode
    {
        public bool IsLocked;
        public abstract bool Evaluate(Context context);
        public abstract string ToReadableString();
        public abstract Expression BuildExpression(ParameterExpression contextParam);
    }

    #region Comparison
    [Serializable]
    public sealed class GreaterNode : ConditionNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public override bool Evaluate(Context context) => (Left?.Evaluate(context) ?? 0f) > (Right?.Evaluate(context) ?? 0f);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "0"} > {Right?.ToReadableString() ?? "0"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(0f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.GreaterThan(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class LessNode : ConditionNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public override bool Evaluate(Context context) => (Left?.Evaluate(context) ?? 0f) < (Right?.Evaluate(context) ?? 0f);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "0"} < {Right?.ToReadableString() ?? "0"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(0f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.LessThan(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class GreaterOrEqualNode : ConditionNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public override bool Evaluate(Context context) => (Left?.Evaluate(context) ?? 0f) >= (Right?.Evaluate(context) ?? 0f);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "0"} >= {Right?.ToReadableString() ?? "0"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(0f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.GreaterThanOrEqual(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class LessOrEqualNode : ConditionNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public override bool Evaluate(Context context) => (Left?.Evaluate(context) ?? 0f) <= (Right?.Evaluate(context) ?? 0f);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "0"} <= {Right?.ToReadableString() ?? "0"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(0f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.LessThanOrEqual(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class EqualNode : ConditionNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public override bool Evaluate(Context context) => Mathf.Approximately(Left?.Evaluate(context) ?? 0f, Right?.Evaluate(context) ?? 0f);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "0"} == {Right?.ToReadableString() ?? "0"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(0f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(0f);
            var approxMethod = typeof(Mathf).GetMethod("Approximately", new[] { typeof(float), typeof(float) });
            return Expression.Call(approxMethod, leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class NotEqualNode : ConditionNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public override bool Evaluate(Context context) => !Mathf.Approximately(Left?.Evaluate(context) ?? 0f, Right?.Evaluate(context) ?? 0f);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "0"} != {Right?.ToReadableString() ?? "0"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(0f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(0f);
            var approxMethod = typeof(Mathf).GetMethod("Approximately", new[] { typeof(float), typeof(float) });
            return Expression.Not(Expression.Call(approxMethod, leftExp, rightExp));
        }
    }

    #endregion
    #region Logic

    [Serializable]
    public sealed class AndNode : ConditionNode
    {
        [SerializeReference] public ConditionNode Left;
        [SerializeReference] public ConditionNode Right;

        public override bool Evaluate(Context context) => (Left?.Evaluate(context) ?? false) && (Right?.Evaluate(context) ?? false);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "false"} and {Right?.ToReadableString() ?? "false"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(false);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(false);
            return Expression.AndAlso(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class OrNode : ConditionNode
    {
        [SerializeReference] public ConditionNode Left;
        [SerializeReference] public ConditionNode Right;

        public override bool Evaluate(Context context) => (Left?.Evaluate(context) ?? false) || (Right?.Evaluate(context) ?? false);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "false"} or {Right?.ToReadableString() ?? "false"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(false);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(false);
            return Expression.OrElse(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class NotNode : ConditionNode
    {
        [SerializeReference] public ConditionNode Node;

        public override bool Evaluate(Context context) => !(Node?.Evaluate(context) ?? false);
        public override string ToReadableString() => $"(not {Node?.ToReadableString() ?? "false"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var exp = Node != null ? Node.BuildExpression(contextParam) : Expression.Constant(false);
            return Expression.Not(exp);
        }
    }

    #endregion
    #region Variable

    [Serializable]
    public sealed class ConditionVariableNode : ConditionNode
    {
        public ConditionVariable Variable;

        public ConditionVariableNode() { }
        public ConditionVariableNode(ConditionVariable var) => Variable = var;
        public override bool Evaluate(Context context) => ConditionResolver.Resolve(Variable, context);
        public override string ToReadableString() => Variable.ToString();

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var resolveMethod = typeof(ConditionResolver).GetMethod("Resolve", BindingFlags.Public | BindingFlags.Static);
            var variableConstant = Expression.Constant(Variable);
            return Expression.Call(resolveMethod, variableConstant, contextParam);
        }
    }

    public static class ConditionResolver
    {
        public static bool Resolve(ConditionVariable variable, Context context)
        {
            if (context.Target == null) return false; 
            return variable switch
            {
                IsDead => context.Target.HealthComponent.IsDead,
                IsCorrupted => context.Target.Corruption.Stability == 0,
                IsBoss => false,
                IsFullHealth => Mathf.Approximately(context.Target.HealthComponent.Health, context.Target.HealthComponent.MaxHealth),
                _ => false
            };
        }
    }

    #endregion

    [Serializable]
    public struct Condition
    {
        [SerializeReference] private ConditionNode rootNode;
        private Func<Context, bool> _cachedFunc;

        public Condition(ConditionNode node)
        {
            rootNode = node;
            _cachedFunc = null;
        }

        public void Compile()
        {
            if (rootNode == null)
            {
                _cachedFunc = context => false;
                return;
            }
            var contextParam = Expression.Parameter(typeof(Context), "context");
            var body = rootNode.BuildExpression(contextParam);
            var lambda = Expression.Lambda<Func<Context, bool>>(body, contextParam);
            _cachedFunc = lambda.Compile();
        }

        public bool Evaluate(Context context)
        {
            if (_cachedFunc == null) Compile();
            return _cachedFunc(context);
        }

        public readonly string ToReadableString() => rootNode != null ? rootNode.ToReadableString() : "None";
    }

    public enum ConditionVariable
    {
        IsDead, IsCorrupted, IsBoss, IsFullHealth,
    }
}