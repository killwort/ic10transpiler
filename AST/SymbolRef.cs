using System;
using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class SymbolRef : Expr
{
    internal SymbolRef(Token srcToken, string name) : base(srcToken)
    {
        Name = name;
    }

    internal string Name;
    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        var symbol = scope.LookupSymbol(this);
        if (symbol == null) yield return new Error(Line, Column, $"Reference to undefined symbol {Name}", ErrorType.Semantic);
    }

    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        var symbol = scope.LookupSymbol(this);
        if (symbol is VarDeclaration decl)
            yield return new Instruction(this, "move",Acc0Operand.Instance, new RegOperand(decl));
        else if (symbol is Define def)
            yield return new Instruction(this, "move",Acc0Operand.Instance, new DefinitionOperand(def.Name));
        else if (symbol is Argument arg)
            yield return new Instruction(this, "move",Acc0Operand.Instance, new RegOperand(arg));
        else throw new InvalidOperationException();
    }
}
