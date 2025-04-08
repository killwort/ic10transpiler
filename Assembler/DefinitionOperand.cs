namespace Ic10Transpiler.Assembler;

internal class DefinitionOperand : Operand
{
    public DefinitionOperand(string name)
    {
        Name = name;
    }

    public string Name;
    public override string ToString() => Name;
}