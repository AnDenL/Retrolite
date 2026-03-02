namespace CalculatingSystem
{
    using System;
    using UnityEngine;
    using static CalculatingSystem.StatVariable;
    using Creatures;
    using Random = UnityEngine.Random;
    using System.Linq.Expressions;

    #region Numbers
    [Serializable]
    public abstract class FormulaNode
    {
        [HideInInspector] public bool IsLocked;

        public abstract float Evaluate(Context context);
        public abstract string ToReadableString();
        public abstract bool IsConstant();
        public virtual int GetNodeCount() => 1;
        public abstract Expression BuildExpression(ParameterExpression contextParam);
    }

    [Serializable]
    public sealed class ConstantNode : FormulaNode
    {
        public float Value;
        public override bool IsConstant() => true;

        public ConstantNode() => Value = 0;

        public ConstantNode(float value) => Value = value;
        public override float Evaluate(Context context) => Value;
        public override string ToReadableString() => Value.ToString();
        public override Expression BuildExpression(ParameterExpression contextParam) => Expression.Constant(Value);
    }

    [Serializable]
    public sealed class RandomNode : FormulaNode
    {
        [SerializeReference]
        public FormulaNode A, B;
        public override bool IsConstant() => false;

        public RandomNode() {}

        public RandomNode(FormulaNode a, FormulaNode b)
        {
            A = a;
            B = b;
        }

        public override float Evaluate(Context context) => Random.Range(A.Evaluate(context), B.Evaluate(context));
        public override string ToReadableString() => $"Rand({A.ToReadableString()},{B.ToReadableString()})";
        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var rangeMethod = typeof(Random).GetMethod("Range", new[] { typeof(float), typeof(float) });

            var minExp = A != null ? A.BuildExpression(contextParam) : Expression.Constant(0f);
            var maxExp = B != null ? B.BuildExpression(contextParam) : Expression.Constant(0f);

            return Expression.Call(rangeMethod, minExp, maxExp);
        }
    }

    #endregion
    #region Functions

    [Serializable]
    public sealed class AbsoluteNode : FormulaNode
    {
        [SerializeReference]
        public FormulaNode Node;
        public override bool IsConstant() => true;

        public AbsoluteNode() => Node = new ConstantNode(0);

        public AbsoluteNode(FormulaNode node) => Node = node;
        public override float Evaluate(Context context) => Mathf.Abs(Node.Evaluate(context));
        public override string ToReadableString() => "|" + Node.ToReadableString() + "|";
        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var cosMethod = typeof(Mathf).GetMethod("Abs", new[] { typeof(float) });
            var innerExpression = Node != null ? Node.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.Call(cosMethod, innerExpression);
        }
    }

    [Serializable]
    public sealed class SinNode : FormulaNode
    {
        [SerializeReference]
        public FormulaNode Node;
        public override bool IsConstant() => Node.IsConstant();

        public SinNode() => Node = new ConstantNode(0);

        public SinNode(FormulaNode value) => Node = value;
        public override float Evaluate(Context context) => Mathf.Sin(Node.Evaluate(context));
        public override string ToReadableString() => "Sin(" + Node.ToReadableString() + ")";
        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var cosMethod = typeof(Mathf).GetMethod("Sin", new[] { typeof(float) });
            var innerExpression = Node != null ? Node.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.Call(cosMethod, innerExpression);
        }
    }

    [Serializable]
    public sealed class CosNode : FormulaNode
    {
        [SerializeReference]
        public FormulaNode Node;
        public override bool IsConstant() => Node.IsConstant();

        public CosNode() => Node = new ConstantNode(0);

        public CosNode(FormulaNode value) => Node = value;
        public override float Evaluate(Context context) => Mathf.Cos(Node.Evaluate(context));
        public override string ToReadableString() => "Cos(" + Node.ToReadableString() + ")";
        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var cosMethod = typeof(Mathf).GetMethod("Cos", new[] { typeof(float) });
            var innerExpression = Node != null ? Node.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.Call(cosMethod, innerExpression);
        }
    }

    #endregion
    #region Variables

    [Serializable]
    public sealed class VariableNode : FormulaNode
    {
        public StatVariable Variable;
        public override bool IsConstant() => false;

        public VariableNode() { }
        public VariableNode(StatVariable var) => Variable = var;
        public override float Evaluate(Context context) => VariableResolver.Resolve(Variable, context);
        public override string ToReadableString() => Variable.ToString();
        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var resolveMethod = typeof(VariableResolver).GetMethod("Resolve");
            var variableConstant = Expression.Constant(Variable);
            
            return Expression.Call(resolveMethod, variableConstant, contextParam);
        }
    }

    public static class VariableResolver
    {
        public static float Resolve(StatVariable variable, Context context)
        {
            return variable switch
            {
                PH => context.Owner != null ? context.Owner.HealthComponent.GetHealthPercent() : 0,
                H => context.Target != null ? context.Target.HealthComponent.GetHealthPercent() : Break(variable, context),
                T => context.Bullet != null ? context.Bullet.GetLifetime() : Break(variable, context),
                E => context.Gun != null ? context.Gun.Data.Echo : Break(variable, context),
                D => context.Bullet != null ? context.Bullet.GetDistanceTravelled() : Break(variable, context),
                P => Vector2.Distance(PlayerController.Player.transform.position, context.Bullet != null ? context.Bullet.transform.position : Vector3.zero),
                A => context.Bullet != null ? context.Bullet.Number : Break(variable, context),
                M => PlayerController.Player.Resources.Get(ResourceType.Money).Count / 100,
                SP => context.Bullet != null ? context.Bullet.Speed : Break(variable, context),
                S => context.Bullet != null ? context.Bullet.Scale : Break(variable, context),
                R => context.Bullet != null ? context.Bullet.Spread : Break(variable, context) * Mathf.Deg2Rad,
                DT => context.Bullet != null ? context.Bullet.GetDestroyTime() : Break(variable, context),
                V => context.Target != null ? context.Target.Rb.velocity.magnitude : Break(variable, context),
                DR => Utilities.CalculateHomingAngle(context),
                _ => 0f
            };
        }

        public static void FillVariables(Context context, Span<float> resolvedVars)
        {
            resolvedVars[(int)PH] = context.Owner != null ? context.Owner.HealthComponent.GetHealthPercent() : 0;
            resolvedVars[(int)H] = context.Target != null ? context.Target.HealthComponent.GetHealthPercent() : 0;
            resolvedVars[(int)T] = context.Bullet != null ? context.Bullet.GetLifetime() : 0;
            resolvedVars[(int)E] = context.Gun != null ? context.Gun.Data.Echo : 0;
            resolvedVars[(int)D] = context.Bullet != null ? context.Bullet.GetDistanceTravelled() : 0;
            resolvedVars[(int)P] = Vector2.Distance(PlayerController.Player.transform.position, context.Bullet == null ? context.Bullet.transform.position : Vector3.zero);
            resolvedVars[(int)A] = context.Bullet != null ? context.Bullet.Number : 0;
            resolvedVars[(int)M] = PlayerController.Player.Resources.Get(ResourceType.Money).Count / 100;
            resolvedVars[(int)SP] = context.Bullet != null ? context.Bullet.Speed : 0;
            resolvedVars[(int)S] = context.Bullet != null ? context.Bullet.Scale : 0;
            resolvedVars[(int)R] = context.Bullet != null ? context.Bullet.Spread : 0 * Mathf.Deg2Rad;
            resolvedVars[(int)DT] = context.Bullet != null ? context.Bullet.GetDestroyTime() : 0;
            resolvedVars[(int)V] = context.Target != null ? context.Target.Rb.velocity.magnitude : 0;
            resolvedVars[(int)DR] = Utilities.CalculateHomingAngle(context);
        }

        public static float Break(StatVariable variable, Context context)
        {
            return 0; //Add weapon breaking in future
        }
    }

    public enum StatVariable { PH, H, T, E, D, P, A, M, SP, S, R, DT, V, DR }

    #endregion
    #region Operators

    [Serializable]
    public sealed class AddNode : FormulaNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public AddNode()
        {
            Left = new ConstantNode(0);
            Right = new ConstantNode(0);
        }
        public AddNode(FormulaNode node)
        {
            Left = node;
            Right = new ConstantNode(0);
        }
        public AddNode(FormulaNode left, FormulaNode right)
        {
            Left = left;
            Right = right;
        }

        public override float Evaluate(Context context) => Left.Evaluate(context) + Right.Evaluate(context);
        
        public override bool IsConstant() => (Left?.IsConstant() ?? true) && (Right?.IsConstant() ?? true);
        public override int GetNodeCount() => 1 + (Left?.GetNodeCount() ?? 0) + (Right?.GetNodeCount() ?? 0);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "0"} + {Right?.ToReadableString() ?? "0"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(0f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.Add(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class SubtractNode : FormulaNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public SubtractNode()
        {
            Left = new ConstantNode(0);
            Right = new ConstantNode(0);
        }
        public SubtractNode(FormulaNode node)
        {
            Left = node;
            Right = new ConstantNode(0);
        }
        public SubtractNode(FormulaNode left, FormulaNode right)
        {
            Left = left;
            Right = right;
        }

        public override float Evaluate(Context context) => Left.Evaluate(context) - Right.Evaluate(context);

        public override bool IsConstant() => (Left?.IsConstant() ?? true) && (Right?.IsConstant() ?? true);
        public override int GetNodeCount() => 1 + (Left?.GetNodeCount() ?? 0) + (Right?.GetNodeCount() ?? 0);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "0"} - {Right?.ToReadableString() ?? "0"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(0f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(0f);
            return Expression.Subtract(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class MultiplyNode : FormulaNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public MultiplyNode()
        {
            Left = new ConstantNode(1);
            Right = new ConstantNode(1);
        }
        public MultiplyNode(FormulaNode node)
        {
            Left = node;
            Right = new ConstantNode(1);
        }
        public MultiplyNode(FormulaNode left, FormulaNode right)
        {
            Left = left;
            Right = right;
        }

        public override float Evaluate(Context context) => Left.Evaluate(context) * Right.Evaluate(context);

        public override bool IsConstant() => (Left?.IsConstant() ?? true) && (Right?.IsConstant() ?? true);
        public override int GetNodeCount() => 1 + (Left?.GetNodeCount() ?? 0) + (Right?.GetNodeCount() ?? 0);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "1"} * {Right?.ToReadableString() ?? "1"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(1f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(1f);
            return Expression.Multiply(leftExp, rightExp);
        }
    }

    [Serializable]
    public sealed class DivideNode : FormulaNode
    {
        [SerializeReference] public FormulaNode Left;
        [SerializeReference] public FormulaNode Right;

        public DivideNode()
        {
            Left = new ConstantNode(1);
            Right = new ConstantNode(1);
        }
        public DivideNode(FormulaNode node)
        {
            Left = node;
            Right = new ConstantNode(1);
        }
        public DivideNode(FormulaNode left, FormulaNode right)
        {
            Left = left;
            Right = right;
        }

        public override float Evaluate(Context context) => Left.Evaluate(context) / Right.Evaluate(context);

        public override bool IsConstant() => (Left?.IsConstant() ?? true) && (Right?.IsConstant() ?? true);
        public override int GetNodeCount() => 1 + (Left?.GetNodeCount() ?? 0) + (Right?.GetNodeCount() ?? 0);
        public override string ToReadableString() => $"({Left?.ToReadableString() ?? "1"} / {Right?.ToReadableString() ?? "1"})";

        public override Expression BuildExpression(ParameterExpression contextParam)
        {
            var leftExp = Left != null ? Left.BuildExpression(contextParam) : Expression.Constant(1f);
            var rightExp = Right != null ? Right.BuildExpression(contextParam) : Expression.Constant(1f);
            return Expression.Divide(leftExp, rightExp);
        }
    }
    #endregion
}