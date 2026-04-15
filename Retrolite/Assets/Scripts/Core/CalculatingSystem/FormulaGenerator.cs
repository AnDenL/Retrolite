namespace CalculatingSystem
{
    using System;
    public static class FormulaGenerator
    {
        public static FormulaNode GenerateRandomFormula(GameRandom rnd, int depth = 0, int maxDepth = 4)
        {
            if (depth >= maxDepth)
                return RandomLeaf(rnd);

            int choice = rnd.Range(0, 15);
            return choice switch
            {
                0 => RandomConstant(rnd),
                1 => RandomVariable(rnd),
                2 => new AddNode(
                        GenerateRandomFormula(rnd, depth + 1, maxDepth),
                        GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                3 => new SubtractNode(
                    GenerateRandomFormula(rnd, depth + 1, maxDepth),
                    GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                4 => new MultiplyNode(
                    GenerateRandomFormula(rnd, depth + 1, maxDepth),
                    GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                5 => new DivideNode(
                    GenerateRandomFormula(rnd, depth + 1, maxDepth),
                    GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                6 => new AbsoluteNode(GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                7 => new SinNode(GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                8 => new CosNode(GenerateRandomFormula(rnd, depth + 1, maxDepth)),
                _ => RandomConstant(rnd)
            };
        }

        private static FormulaNode RandomLeaf(GameRandom rnd)
        {
            return rnd.Range(0, 2) == 0 ? RandomConstant(rnd) : RandomVariable(rnd);
        }

        private static ConstantNode RandomConstant(GameRandom rnd)
        {
            float value = (float)Math.Round(rnd.Value * 50 - 25, 2);
            return new ConstantNode(value);
        }

        private static VariableNode RandomVariable(GameRandom rnd)
        {
            Array values = Enum.GetValues(typeof(StatVariable));
            StatVariable randomVar = (StatVariable)values.GetValue(rnd.Range(0, values.Length));
            return new VariableNode(randomVar);
        }
    }
}