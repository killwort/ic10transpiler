namespace Ic10Transpiler.AST;

internal interface ISymbol : ISourceMappedElement
{
    string Name { get; }
}
