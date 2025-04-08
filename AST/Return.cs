using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class Return : Statement
{
    internal Return(Token srcToken, Expr returnValue) : base(srcToken)
    {
        ReturnValue = returnValue;
    }

    internal Expr ReturnValue;

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        if (!(scope is FunctionDeclaration))
            yield return new Error(Line, Column, "Return statement outside of function", ErrorType.Semantic);
    }

    public override bool IsAllBranchesHasReturn() => true;
    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        foreach (var l in ReturnValue.Emit(scope)) yield return l;
        //Return value already in r0, no need to do extra move
        yield return new Instruction(this, "j", new LabelOperand(((FunctionDeclaration)scope).ExitLabel));
    }
}
