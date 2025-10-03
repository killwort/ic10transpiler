using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class Status : BuiltinFunction
{
    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "value")
    };

    public override string Name => "status";
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        Operand valueOperand = Acc0Operand.Instance;
        if (IsConstOrDefine(scope.Root, call.Arguments[0]))
            valueOperand = ToOperand(call.Arguments[0], scope);
        else
            foreach (var l in call.Arguments[0].Emit(scope))
                yield return l;
        yield return new Instruction(call, "s",  new DeviceOperand("db"), new DefinitionOperand("Setting"), valueOperand);
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        yield break;
    }
}
