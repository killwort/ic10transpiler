namespace Ic10Transpiler.Assembler;

internal class LabelOperand : Operand
{
    public Label Target;

    public LabelOperand(Label target)
    {
        Target = target;
    }

    public override string ToString() => Target.Name;
    
    public override bool Equals(Operand other)
    {
        return other is LabelOperand op && op.Target.Name == Target.Name;
    }
}
