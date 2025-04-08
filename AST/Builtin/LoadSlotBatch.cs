using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class LoadSlotBatch : BuiltinFunction
{
    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "typeHash"),
        new Argument(new Token(), "slot"),
        new Argument(new Token(), "variable"),
        new Argument(new Token(), "mode")
    };

    public override string Name => "getSlotBatch";
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        Operand slotIdOperand = Acc0Operand.Instance;
        if (IsConstOrDefine(scope.Root, call.Arguments[1]))
            slotIdOperand = ToOperand(call.Arguments[1]);
        else
            foreach (var l in call.Arguments[1].Emit(scope))
                yield return l;
        yield return new Instruction(call, "lbs", Acc0Operand.Instance, ToOperand(call.Arguments[0]), slotIdOperand, ToOperand(call.Arguments[2]), ToOperand(call.Arguments[3]));
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        if (!IsConstOrDefine(scope.Root, call.Arguments[0])) yield return new Error(call.Line, call.Column, "TypeHash argument should be constant", ErrorType.Semantic);
        if (!IsConstOrDefine(scope.Root, call.Arguments[2])) yield return new Error(call.Line, call.Column, "Variable argument should be constant", ErrorType.Semantic);
        if (!IsConstOrDefine(scope.Root, call.Arguments[3])) yield return new Error(call.Line, call.Column, "Mode argument should be constant", ErrorType.Semantic);
    }
}