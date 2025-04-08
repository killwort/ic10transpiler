namespace Ic10Transpiler.Assembler;

internal class LabelOperand : Operand
{
    public Label Target;

    public LabelOperand(Label target)
    {
        Target = target;
    }

    public override string ToString() => Target.Name;
}
