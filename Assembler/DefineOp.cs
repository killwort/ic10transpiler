using Ic10Transpiler.AST;

namespace Ic10Transpiler.Assembler;

internal class DefineOp : Op
{
    public DefineOp(SourceMappedElement source, string name, string value):base(source)
    {
        Name = name;
        Value = value;
    }

    public string Name;
    public string Value;
    public override string ToString() => $"define {Name} {Value}";
}
