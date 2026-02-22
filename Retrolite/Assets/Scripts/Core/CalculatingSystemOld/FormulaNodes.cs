namespace CalculatingSystem
{
    using System;
    using UnityEngine;
    using static CalculatingSystem.Operator;
    using static CalculatingSystem.StatVariable;
    using Creatures;
    using Random = UnityEngine.Random;

    [Serializable]
    public abstract class FormulaNode
    {
        public abstract float Evaluate(Context context);
        public abstract string ToReadableString();
        public abstract bool IsConstant();
        public abstract Func<Context, float> Bake();
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
        public override Func<Context, float> Bake() => context => Value;
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
        public override Func<Context, float> Bake()
        {
            var a = A.Bake(); 
            var b = B.Bake();
            return context => Random.Range(a(context), b(context));
        }
    }

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
        public override Func<Context, float> Bake()
        {
            var f = Node.Bake();
            return context => Mathf.Abs(f(context)); 
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
        public override Func<Context, float> Bake()
        {
            var f = Node.Bake();
            return context => Mathf.Sin(f(context)); 
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
        public override Func<Context, float> Bake()
        {
            var f = Node.Bake();
            return context => Mathf.Cos(f(context)); 
        } 
    }

    [Serializable]
    public sealed class VariableNode : FormulaNode
    {
        public StatVariable Variable;
        public override bool IsConstant() => false;

        public VariableNode() { }
        public VariableNode(StatVariable var) => Variable = var;
        public override float Evaluate(Context context) => VariableResolver.Resolve(Variable, context);
        public override string ToReadableString() => Variable.ToString();
        public override Func<Context, float> Bake() => context => Variable switch
        {
            PH => context.Owner != null ? context.Owner.HealthComponent.GetHealthPercent() : 0,
            H => context.Target != null ? context.Target.HealthComponent.GetHealthPercent() : 0,
            T => context.Bullet != null ? context.Bullet.GetLifetime() : 0,
            E => context.Gun != null ? context.Gun.Data.Echo : 0,
            D => context.Bullet != null ? context.Bullet.GetDistanceTravelled() : 0,
            P => Vector2.Distance(PlayerController.Player.transform.position, context.Bullet ? context.Bullet.transform.position : Vector3.zero),
            A => context.Bullet != null ? context.Bullet.Number : 0,
            M => PlayerController.Player.Resources.Get(ResourceType.Money).Count / 100,
            SP => context.Bullet != null ? context.Bullet.Speed : 0,
            S => context.Bullet != null ? context.Bullet.Scale : 0,
            R => context.Bullet != null ? context.Bullet.Spread : 0,
            DT => context.Bullet != null ? context.Bullet.GetDestroyTime() : 0,
            V => context.Target != null ? context.Target.Rb.velocity.magnitude : 0,
            DR => Utilities.CalculateHomingAngle(context),
            _ => 0f
        };
    }

    [Serializable]
    public sealed class Expression : FormulaNode
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

        public override float Evaluate(Context context)
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

        public override Func<Context, float> Bake() 
        {
            var a = Left.Bake();
            var b = Right.Bake();
            return Operation switch
            {
                Add => context => a(context) + b(context),
                Subtract => context => a(context) - b(context),
                Multiply => context => a(context) * b(context),
                Divide => context => a(context) / b(context),
                _ => context => 0
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
                P => Vector2.Distance(PlayerController.Player.transform.position, context.Bullet == null ? context.Bullet.transform.position : Vector3.zero),
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

    public enum StatVariable
    {
        PH,
        H,
        T,
        E,
        D,
        P,
        A,
        M,
        SP,
        S,
        R,
        DT,
        V,
        DR
    }

    [Serializable]
    public struct Formula
    {
        [SerializeReference] private FormulaNode rootNode;
        private Func<Context, float> _cachedFunc;

        public Formula(FormulaNode node)
        {
            rootNode = node;
            _cachedFunc = rootNode.Bake();
        }

        public float Evaluate(Context context)
        {
            _cachedFunc ??= rootNode.Bake();
            return _cachedFunc(context);
        } 

        public readonly string ToReadableString() => rootNode != null ? rootNode.ToReadableString() : "None";
        public readonly bool IsConstant() => rootNode == null || rootNode.IsConstant();
    }
}