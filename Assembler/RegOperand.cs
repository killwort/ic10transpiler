using Ic10Transpiler.AST;

namespace Ic10Transpiler.Assembler;

internal class RegOperand : Operand
{
    public readonly IRegister Source;

    public RegOperand(IRegister source)
    {
        Source = source;
        RegNumber = source.AssignedRegister;
    }

    public int RegNumber;
    public override string ToString() => "r" + RegNumber;
    
    public override bool Equals(Operand other)
    {
        return other is RegOperand op && op.RegNumber == RegNumber;
    }
}
