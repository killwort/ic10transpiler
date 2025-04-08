using Ic10Transpiler.AST;

namespace Ic10Transpiler.Assembler;

internal abstract class Op
{
    protected Op(SourceMappedElement source)
    {
        Source = source;
    }

    public readonly SourceMappedElement Source;
}
