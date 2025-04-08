using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class Symbol : SourceMappedElement, ISymbol
{
    public string Name { get; }

    internal Symbol(Token srcToken, string name) : base(srcToken)
    {
        Name = name;
    }
}