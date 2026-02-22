namespace FormulaSystem
{
    using static OpCode;
    
    using System;

    public static class Calculator
    {
        public static unsafe float Evaluate(byte[] bytecode, ReadOnlySpan<float> variables)
        {
            if (bytecode == null || bytecode.Length == 0) return 0;

            float* stack = stackalloc float[32]; 
            int sp = 0;
            int ip = 0;

            while (ip < bytecode.Length)
            {
                OpCode op = (OpCode)bytecode[ip++];

                switch (op)
                {
                    case PushConst:
                        fixed (byte* p = &bytecode[ip]) {
                            stack[sp++] = *(float*)p;
                        }
                        ip += 4;
                        break;
                    case PushVar:
                        fixed (byte* p = &bytecode[ip]) {
                            int varIndex = *(int*)p;
                            stack[sp++] = variables[varIndex];
                        }
                        ip += 4;
                        break;
                    case Add:
                        stack[sp - 2] = stack[sp - 2] + stack[sp - 1];
                        sp--;
                        break;
                    case Sub:
                        stack[sp - 2] = stack[sp - 2] - stack[sp - 1];
                        sp--;
                        break;
                    case Mul:
                        stack[sp - 2] = stack[sp - 2] * stack[sp - 1];
                        sp--;
                        break;
                    case Div:
                        stack[sp - 2] = stack[sp - 2] / stack[sp - 1];
                        sp--;
                        break;
                    case Call:
                        fixed (byte* p = &bytecode[ip]) {
                            byte funcId = *p;
                            ip += 1;
                            
                            switch (funcId)
                            {
                                case 0:
                                {
                                    float b = stack[--sp];
                                    float a = stack[--sp];
                                    stack[sp++] = Math.Max(a, b);
                                    break;
                                }
                                case 1:
                                {
                                    float b = stack[--sp];
                                    float a = stack[--sp];
                                    stack[sp++] = Math.Min(a, b);
                                    break;
                                }
                                case 2:
                                {
                                    float a = stack[--sp];
                                    stack[sp++] = (float)Math.Sin(a);
                                    break;
                                }
                                case 3:
                                {
                                    float a = stack[--sp];
                                    stack[sp++] = (float)Math.Cos(a);
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            return stack[0];
        }
    }

public enum TokenType 
{ 
    Number, Variable, Operator, Function, LeftParen, RightParen 
}

public struct PuzzleToken 
{
    public TokenType Type;
    public float Value;
    public byte Id;   
    public char Op;
}

public enum OpCode : byte
{
    PushConst = 0, 
    PushVar = 1,
    Add = 2,
    Sub = 3,
    Mul = 4,
    Div = 5,
    Call = 6 
}

public enum Function : byte
{
    Max,
    Min,
    Sin,
    Cos,
    Abs
}
}