using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class BreakStatement : Statement
{
    internal BreakStatement(Token srcToken) : base(srcToken)
    {
    }

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        var loop= scope.LoopStack.Peek();
        if (loop == null)
            yield return new Error(Line, Column, "Break statement outside of loop", ErrorType.Semantic);

    }

    public override bool IsAllBranchesHasReturn() => false;
    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        var loop = scope.LoopStack.Peek();
        yield return new Instruction(this, "j", new LabelOperand(loop.ExitLabel));
    }
}
