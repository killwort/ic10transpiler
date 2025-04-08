namespace Ic10Transpiler.AST;

internal interface ISourceMappedElement
{
    int Line { get; }
    int Column { get; }
}