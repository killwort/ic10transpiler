using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class Save : BuiltinFunction
{
    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "device"),
        new Argument(new Token(), "variable"),
        new Argument(new Token(), "value")
    };

    public override string Name => "set";
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        Operand valueOperand = Acc0Operand.Instance;
        if (IsConstOrDefine(scope.Root, call.Arguments[2]))
            valueOperand = ToOperand(call.Arguments[2]);
        else
            foreach (var l in call.Arguments[2].Emit(scope))
                yield return l;
        yield return new Instruction(call, "s", ToOperand(call.Arguments[0]), valueOperand);
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        if (!IsConstOrDefine(scope.Root, call.Arguments[0])) yield return new Error(call.Line, call.Column, "Device argument should be constant", ErrorType.Semantic);
        if (!IsConstOrDefine(scope.Root, call.Arguments[1])) yield return new Error(call.Line, call.Column, "Variable argument should be constant", ErrorType.Semantic);
    }
}