using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class LogicalConstant : LogicalExpression
{
    public bool Value { get; }

    internal LogicalConstant(Token srcToken, bool value) : base(srcToken)
    {
        Value = value;
    }

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        yield break;
    }

    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        yield return new Instruction(this, "move", Acc0Operand.Instance, new ImmediateOperand(Value ? 1 : 0));
    }
}
