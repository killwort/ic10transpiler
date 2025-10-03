namespace Ic10Transpiler.Assembler;

internal class Acc1Operand : Operand
{
    private static Operand _instance = new Acc1Operand();
    public static Operand Instance => _instance;

    private Acc1Operand()
    {
    }

    public override string ToString() => "r1";
    
    public override bool Equals(Operand other)
    {
        return other is Acc0Operand;
    }
}
