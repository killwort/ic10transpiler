using Ic10Transpiler.AST;

namespace Ic10Transpiler.Assembler;

internal class Label : Op
{
    public Label(SourceMappedElement source, string name) : base(source)
    {
        Name = name;
    }

    public string Name;
    public override string ToString() => Name + ":";
}
