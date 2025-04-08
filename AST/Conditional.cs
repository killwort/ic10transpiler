using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class Conditional : Statement
{
    internal LogicalExpression Condition;
    internal Statement True, False;

    internal Conditional(Token srcToken, LogicalExpression condition, Statement @true) : base(srcToken)
    {
        Condition = condition;
        True = @true;
    }

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        foreach (var e in Condition.Validate(scope)) yield return e;
        foreach (var e in True.Validate(scope)) yield return e;
        if (False != null)
            foreach (var e in False.Validate(scope))
                yield return e;
    }

    public override bool IsAllBranchesHasReturn() => True.IsAllBranchesHasReturn() && (False == null || False.IsAllBranchesHasReturn());

    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        var lblEndIf = new Label(this, scope.Root.UniqueLabel() + "_if_end");
        var lblElse = False != null ? new Label(this, scope.Root.UniqueLabel() + "_if_else") : null;
        if (Condition is LogicalConstant lc)
        {
            if (!lc.Value)
                if (False != null)
                    yield return new Instruction(this, "j", new LabelOperand(lblElse));
                else
                    yield return new Instruction(this, "j", new LabelOperand(lblEndIf));
        }
        else
        {
            foreach (var l in Condition.Emit(scope)) yield return l;
            if (False != null)
                yield return new Instruction(this, "beqz", Acc0Operand.Instance, new LabelOperand(lblElse));
            else
                yield return new Instruction(this, "beqz", Acc0Operand.Instance, new LabelOperand(lblEndIf));
        }

        foreach (var l in True.Emit(scope)) yield return l;
        if (False != null)
        {
            yield return new Instruction(this, "j", new LabelOperand(lblEndIf));
            yield return lblElse;
            foreach (var l in False.Emit(scope)) yield return l;
        }

        yield return lblEndIf;
    }
}
