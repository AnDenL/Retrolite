namespace ByteCodeCalculator
{
    using System;
    using static FormulaByte;

    public static class ByteCodeCalculator
    {
        public static float Evaluate(byte[] formula, float[] variables)
        {
            int order = 0;

            float result = 0;

            while (order < formula.Length)
            {
                switch ((FormulaByte)formula[order])
                {
                    case Const:
                        result = BitConverter.ToSingle(formula, order + 1);
                        order += 5;
                        break;
                }
            }

            return result;
        }
    }

    public enum FormulaByte : byte
    {
        Const,
        Variable,
    }
}