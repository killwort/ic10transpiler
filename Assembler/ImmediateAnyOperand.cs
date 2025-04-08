namespace Ic10Transpiler.Assembler;

internal class ImmediateAnyOperand : Operand
{
    public ImmediateAnyOperand(string value)
    {
        Value = value;
    }

    public string Value;
    public override string ToString() => Value;
}