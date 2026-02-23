namespace FormulaSystem
{
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
    public bool IsLocked;
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
    Abs,
    Rand
}
}