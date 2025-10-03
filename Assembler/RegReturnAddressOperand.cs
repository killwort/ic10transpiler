namespace Ic10Transpiler.Assembler;

internal class RegReturnAddressOperand : Operand
{
    public override string ToString() => "ra";
    
    public override bool Equals(Operand other)
    {
        return other is RegReturnAddressOperand;
    }
}
