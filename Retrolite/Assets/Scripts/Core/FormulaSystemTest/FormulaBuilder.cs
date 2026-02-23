namespace FormulaSystem
{
    using System;
    using System.Collections.Generic;

    public class BytecodeBuilder
    {
        private List<byte> _buffer = new();
        public BytecodeBuilder PushConst(float value)
        {
            _buffer.Add((byte)OpCode.PushConst);
            _buffer.AddRange(BitConverter.GetBytes(value));
            return this;
        }

        public BytecodeBuilder PushVar(byte varId)
        {
            _buffer.Add((byte)OpCode.PushVar); 
            _buffer.Add(varId);
            return this;
        }
        
        public BytecodeBuilder Add()
        {
            _buffer.Add((byte)OpCode.Add);
            return this;
        }

        public BytecodeBuilder Sub()
        {
            _buffer.Add((byte)OpCode.Sub);
            return this;
        }

        public BytecodeBuilder Mul()
        {
            _buffer.Add((byte)OpCode.Mul);
            return this;
        }

        public BytecodeBuilder Div()
        {
            _buffer.Add((byte)OpCode.Div);
            return this;
        }

        public BytecodeBuilder Call(Function funcId)
        {
            _buffer.Add((byte)OpCode.Call);
            _buffer.Add((byte)funcId);
            return this;
        }

        public byte[] Build()
        {
            return _buffer.ToArray();
        }
    }
}