using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class Load : BuiltinFunction
{
    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "device"),
        new Argument(new Token(), "variable")
    };

    public override string Name => "get";
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        yield return new Instruction(call, "l", Acc0Operand.Instance, ToOperand(call.Arguments[0]), ToOperand(call.Arguments[1]));
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        if (!IsConstOrDefine(scope.Root, call.Arguments[0])) yield return new Error(call.Line, call.Column, "Device argument should be constant", ErrorType.Semantic);
        if (!IsConstOrDefine(scope.Root, call.Arguments[1])) yield return new Error(call.Line, call.Column, "Variable argument should be constant", ErrorType.Semantic);
    }
}
