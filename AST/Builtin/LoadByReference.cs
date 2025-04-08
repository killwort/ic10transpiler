using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class LoadByReference : BuiltinFunction
{
    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "deviceReference"),
        new Argument(new Token(), "variable")
    };

    public override string Name => "getByReference";
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        Operand deviceIdOperand = Acc0Operand.Instance;
        if (IsConstOrDefine(scope.Root, call.Arguments[0]))
            deviceIdOperand = ToOperand(call.Arguments[0]);
        else
            foreach (var l in call.Arguments[0].Emit(scope))
                yield return l;
        yield return new Instruction(call, "ld", Acc0Operand.Instance, deviceIdOperand, ToOperand(call.Arguments[1]));
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        if (!IsConstOrDefine(scope.Root, call.Arguments[1])) yield return new Error(call.Line, call.Column, "Variable argument should be constant", ErrorType.Semantic);
    }
}