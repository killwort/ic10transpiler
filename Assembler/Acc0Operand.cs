namespace Ic10Transpiler.Assembler;

internal class Acc0Operand : Operand
{
    private static Operand _instance = new Acc0Operand();
    public static Operand Instance => _instance;

    private Acc0Operand()
    {
    }

    public override string ToString() => "r0";
}