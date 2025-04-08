using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class SourceMappedElement:ISourceMappedElement
{
    public int Line { get; }
    public int Column { get; }

    internal SourceMappedElement(Token srcToken)
    {
        Line = srcToken.line;
        Column = srcToken.col;
    }
}
