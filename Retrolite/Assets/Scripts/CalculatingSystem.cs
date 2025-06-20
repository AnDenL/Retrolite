public abstract class FormulaNode
{
    public abstract float Evaluate(FormulaContext context);
    public abstract string ToReadableString();
}

public class ConstantNode : FormulaNode
{
    public float Value { get; }

    public ConstantNode(float value) => Value = value;
    public override float Evaluate(FormulaContext context) => Value;
    public override string ToReadableString() => Value.ToString();
}

public class VariableNode : FormulaNode
{
    public StatVariable Variable;
    public VariableNode(StatVariable var) => Variable = var;
    public override float Evaluate(FormulaContext context) => VariableResolver.Resolve(Variable, context);
    public override string ToReadableString() => Variable.ToString();
}

public class Expression : FormulaNode
{
    public FormulaNode Left;
    public FormulaNode Right;
    public Operator Operation;

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
            Operator.Add => a + b,
            Operator.Subtract => a - b,
            Operator.Multiply => a * b,
            Operator.Divide => b == 0 ? 0 : a / b,
            _ => 0
        };
    }

    public override string ToReadableString() =>
        $"({Left.ToReadableString()} {OpToString(Operation)} {Right.ToReadableString()})";

    private string OpToString(Operator op) => op switch
    {
        Operator.Add => "+",
        Operator.Subtract => "-",
        Operator.Multiply => "*",
        Operator.Divide => "/",
        _ => "?"
    };
}

public enum Operator { Add, Subtract, Multiply, Divide }

public struct FormulaContext
{
    public Player Player;
    public HealthBase Health;
    public GunBase Gun;
    public BulletBase Bullet;
    public float Echo;
}

public static class VariableResolver
{
    public static float Resolve(StatVariable variable, FormulaContext context)
    {
        return variable switch
        {
            StatVariable.PlayerHealthPercent => context.Player?.GetHealthPercent() ?? Break(variable, context),
            StatVariable.HealthPercent => context.Health?.GetHealthPercent() ?? Break(variable, context),
            StatVariable.BulletTime => context.Bullet?.GetLifetime() ?? Break(variable, context),
            StatVariable.EchoDamage => context.Echo,
            _ => 0f
        };
    }
    private static float Break(StatVariable variable, FormulaContext context)
    {
        return float.NaN;
    }
}

public enum StatVariable
{
    PlayerHealthPercent,
    HealthPercent,
    BulletTime,
    EchoDamage
}
