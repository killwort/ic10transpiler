using System.Collections.Generic;

namespace Ic10Transpiler.AST
{

    internal interface IFunction : ISymbol
    {
        IEnumerable<Argument> Arguments { get; }
    }
}
