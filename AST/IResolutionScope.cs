using System.Collections.Generic;
using System.Linq;

namespace Ic10Transpiler.AST;

internal interface IResolutionScope
{
    Program Root { get; }
    List<ISymbol> Symtable { get; }
    Stack<LoopStatement> LoopStack { get; }
    ISymbol LookupSymbol(SymbolRef r)
    {
        return Symtable.FirstOrDefault(x => x.Name == r.Name) ?? Root.LookupSymbol(r);
    }

}
