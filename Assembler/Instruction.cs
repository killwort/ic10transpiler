using Ic10Transpiler.AST;

namespace Ic10Transpiler.Assembler;

internal class Instruction : Op
{
    public Instruction(SourceMappedElement source, string opCode, params Operand[] operands):base(source)
    {
        OpCode = opCode;
        Operands = operands;
    }

    public string OpCode;
    public Operand[] Operands;
    public override string ToString() => $"{OpCode} {string.Join<Operand>(' ', Operands)}"; // # {Source.Line}:{Source.Column} {Source.GetType().Name}";
}
