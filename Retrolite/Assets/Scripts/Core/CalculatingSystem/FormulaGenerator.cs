namespace CalculatingSystem
{
    using System;
    public static class FormulaGenerator
    {
        public static FormulaNode GenerateRandomFormula(GameRandom rnd, int depth = 0, int maxDepth = 8)
        {
            if (depth >= maxDepth)
                return RandomLeaf(rnd);

            int choice = rnd.Range(0, 7);
            return choice switch
            {
                0 => RandomConstant(rnd),
                1 => RandomVariable(rnd),
                2 => new Expression(
                        GenerateRandomFormula(rnd, depth + 1, maxDepth),
                        RandomOperator(rnd),
                        GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                3 => new AbsoluteNode(GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                4 => new SinNode(GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                5 => new CosNode(GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                _ => RandomConstant(rnd)
            };
        }

        private static FormulaNode RandomLeaf(GameRandom rnd)
        {
            return rnd.Range(0, 2) == 0 ? RandomConstant(rnd) : RandomVariable(rnd);
        }

        private static ConstantNode RandomConstant(GameRandom rnd)
        {
            float value = (float)Math.Round(rnd.Value * 10 - 5, 2);
            return new ConstantNode(value);
        }

        private static VariableNode RandomVariable(GameRandom rnd)
        {
            Array values = Enum.GetValues(typeof(StatVariable));
            StatVariable randomVar = (StatVariable)values.GetValue(rnd.Range(0, values.Length));
            return new VariableNode(randomVar);
        }

        private static Operator RandomOperator(GameRandom rnd)
        {
            Array values = Enum.GetValues(typeof(Operator));
            return (Operator)values.GetValue(rnd.Range(0, values.Length));
        }
    }
}