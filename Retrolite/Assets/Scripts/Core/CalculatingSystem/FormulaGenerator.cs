namespace CalculatingSystem
{
    using System;
    public static class FormulaGenerator
    {
        private static System.Random rnd = new();

        public static FormulaNode GenerateRandomFormula(int depth = 0, int maxDepth = 3)
        {
            if (depth >= maxDepth)
                return RandomLeaf();

            int choice = rnd.Next(0, 6);
            return choice switch
            {
                0 => RandomConstant(),
                1 => RandomVariable(),
                2 => new Expression(
                        GenerateRandomFormula(depth + 1, maxDepth),
                        RandomOperator(),
                        GenerateRandomFormula(depth + 1, maxDepth)),
                3 => new AbsoluteNode(GenerateRandomFormula(depth + 1, maxDepth)),
                4 => new SinNode(GenerateRandomFormula(depth + 1, maxDepth)),
                5 => new CosNode(GenerateRandomFormula(depth + 1, maxDepth)),
                _ => RandomConstant()
            };
        }

        private static FormulaNode RandomLeaf()
        {
            return rnd.Next(0, 2) == 0 ? RandomConstant() : RandomVariable();
        }

        private static ConstantNode RandomConstant()
        {
            float value = (float)Math.Round(rnd.NextDouble() * 10 - 5, 2);
            return new ConstantNode(value);
        }

        private static VariableNode RandomVariable()
        {
            Array values = Enum.GetValues(typeof(StatVariable));
            StatVariable randomVar = (StatVariable)values.GetValue(rnd.Next(values.Length));
            return new VariableNode(randomVar);
        }

        private static Operator RandomOperator()
        {
            Array values = Enum.GetValues(typeof(Operator));
            return (Operator)values.GetValue(rnd.Next(values.Length));
        }
    }
}