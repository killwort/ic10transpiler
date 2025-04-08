using System.Collections.Generic;
using System.Globalization;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class ConstValue : Expr
{
    internal ConstValue(Token srcToken, string value) : base(srcToken)
    {
        Value = value;
    }

    internal string Value;
    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        yield break;
    }

    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        yield return new Instruction(this, "move", Acc0Operand.Instance, new ImmediateOperand(float.Parse(Value, NumberStyles.Any, CultureInfo.InvariantCulture)));
    }
}
