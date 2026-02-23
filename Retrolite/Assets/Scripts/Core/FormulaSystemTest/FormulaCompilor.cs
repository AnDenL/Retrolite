namespace FormulaSystem
{
    using System.Collections.Generic;

    public static class FormulaCompiler
    {
        public static byte[] Compile(List<PuzzleToken> puzzles)
        {
            var builder = new BytecodeBuilder();
            var opStack = new Stack<PuzzleToken>();

            foreach (var token in puzzles)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        builder.PushConst(token.Value);
                        break;
                    case TokenType.Variable:
                        builder.PushVar(token.Id);
                        break;
                    case TokenType.Function:
                    case TokenType.LeftParen:
                        opStack.Push(token);
                        break;

                    case TokenType.RightParen:
                        while (opStack.Count > 0 && opStack.Peek().Type != TokenType.LeftParen)
                        {
                            WriteOpToBuilder(opStack.Pop(), builder);
                        }
                        opStack.Pop();

                        if (opStack.Count > 0 && opStack.Peek().Type == TokenType.Function)
                    {
                        WriteOpToBuilder(opStack.Pop(), builder);
                    }
                    break;

                case TokenType.Operator:
                    while (opStack.Count > 0 && opStack.Peek().Type == TokenType.Operator &&
                           GetPrecedence(opStack.Peek().Op) >= GetPrecedence(token.Op))
                    {
                        WriteOpToBuilder(opStack.Pop(), builder);
                    }
                    opStack.Push(token);
                    break;
            }
            }
            while (opStack.Count > 0)
            {
                WriteOpToBuilder(opStack.Pop(), builder);
            }

            return builder.Build();
        }

        private static int GetPrecedence(char op)
        {
            if (op == '+' || op == '-') return 1;
            if (op == '*' || op == '/') return 2;
            return 0;
        }

        private static void WriteOpToBuilder(PuzzleToken token, BytecodeBuilder builder)
        {
            if (token.Type == TokenType.Operator)
            {
                if (token.Op == '+') builder.Add();
                else if (token.Op == '-') builder.Sub();
                else if (token.Op == '*') builder.Mul();
                else if (token.Op == '/') builder.Div();
            }
            else if (token.Type == TokenType.Function)
            {
                builder.Call((Function)token.Id);
            }
        }
        
    }
}