using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class LoopStatement : Statement
{
    internal LoopStatement(Token srcToken, Statement body) : base(srcToken)
    {
        Body = body;
    }

    internal LogicalExpression Precondition, Postcondition;
    internal Statement Body;
    internal Label ExitLabel;

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        if (Precondition != null) foreach (var e in Precondition.Validate(scope)) yield return e;
        if (Postcondition != null) foreach (var e in Postcondition.Validate(scope)) yield return e;
        scope.LoopStack.Push(this);
        foreach (var e in Body.Validate(scope)) yield return e;
        scope.LoopStack.Pop();
    }

    public override bool IsAllBranchesHasReturn() => Body.IsAllBranchesHasReturn();

    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        scope.LoopStack.Push(this);
        var lblEntry = new Label(this, scope.Root.UniqueLabel() + "_loop_entry");
        ExitLabel = new Label(this, scope.Root.UniqueLabel() + "_loop_exit");
        yield return lblEntry;
        if (Precondition != null)
        {
            if (Precondition is LogicalConstant lc)
            {
                if (!lc.Value)
                    yield return new Instruction(this, "j", new LabelOperand(ExitLabel));
            }
            else
            {
                foreach (var l in Precondition.Emit(scope)) yield return l;
                yield return new Instruction(this, "beqz", Acc0Operand.Instance, new LabelOperand(ExitLabel));
            }
        }

        foreach (var l in Body.Emit(scope)) yield return l;
        if (Postcondition != null)
        {
            if (Postcondition is LogicalConstant lc)
            {
                if (lc.Value)
                    yield return new Instruction(this, "j", new LabelOperand(lblEntry));
            }
            else
            {
                foreach (var l in Postcondition.Emit(scope)) yield return l;
                yield return new Instruction(this, "bnez", Acc0Operand.Instance, new LabelOperand(lblEntry));
            }
        }
        else yield return new Instruction(this, "j", new LabelOperand(lblEntry));


        yield return ExitLabel;
        scope.LoopStack.Pop();
    }
}
