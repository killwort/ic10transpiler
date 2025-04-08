using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class Assignment : Statement
{
    internal Assignment(Token srcToken, string target, Expr value) : base(srcToken)
    {
        Target = new SymbolRef(srcToken, target);
        Value = value;
    }

    internal SymbolRef Target;
    internal Expr Value;

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        if (!(scope.LookupSymbol(Target) is IRegister)) yield return new Error(Line, Column, "Cannot assign to non-lvalue", ErrorType.Semantic);
        foreach (var e in Value.Validate(scope)) yield return e;
    }

    public override bool IsAllBranchesHasReturn() => false;
    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        foreach (var l in Value.Emit(scope)) yield return l;
        yield return new Instruction(this,"move", new RegOperand((IRegister)scope.LookupSymbol(Target)), Acc0Operand.Instance);
    }
}
