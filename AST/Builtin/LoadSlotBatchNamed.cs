using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class LoadSlotBatchNamed : BuiltinFunction
{
    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "typeHash"),
        new Argument(new Token(), "nameHash"),
        new Argument(new Token(), "slot"),
        new Argument(new Token(), "variable"),
        new Argument(new Token(), "mode")
    };

    public override string Name => "getSlotBatchNamed";
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        Operand slotIdOperand = Acc0Operand.Instance;
        if (IsConstOrDefine(scope.Root, call.Arguments[2]))
            slotIdOperand = ToOperand(call.Arguments[2], scope);
        else
            foreach (var l in call.Arguments[2].Emit(scope))
                yield return l;

        yield return new Instruction(call, "lbns", Acc0Operand.Instance, ToOperand(call.Arguments[0], scope), ToOperand(call.Arguments[1], scope), slotIdOperand, ToOperand(call.Arguments[3], scope), ToOperand(call.Arguments[4], scope));
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        if (!IsConstOrDefine(scope.Root, call.Arguments[0])) yield return new Error(call.Line, call.Column, "TypeHash argument should be constant", ErrorType.Semantic);
        //if (!IsConstOrDefine(scope.Root, call.Arguments[1])) yield return new Error(call.Line, call.Column, "NameHash argument should be constant", ErrorType.Semantic);
        if (!IsConstOrDefine(scope.Root, call.Arguments[3])) yield return new Error(call.Line, call.Column, "Variable argument should be constant", ErrorType.Semantic);
        if (!IsConstOrDefine(scope.Root, call.Arguments[4])) yield return new Error(call.Line, call.Column, "Mode argument should be constant", ErrorType.Semantic);
    }
}
