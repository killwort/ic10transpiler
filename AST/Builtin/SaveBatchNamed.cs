using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class SaveBatchNamed : BuiltinFunction
{
    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "typeHash"),
        new Argument(new Token(), "nameHash"),
        new Argument(new Token(), "variable"),
        new Argument(new Token(), "value")
    };

    public override string Name => "setBatchNamed";
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        Operand valueOperand = Acc0Operand.Instance;
        if (IsConstOrDefine(scope.Root, call.Arguments[3]))
            valueOperand = ToOperand(call.Arguments[3], scope);
        else
            foreach (var l in call.Arguments[3].Emit(scope))
                yield return l;
        yield return new Instruction(call, "sbn", ToOperand(call.Arguments[0], scope), ToOperand(call.Arguments[1], scope), ToOperand(call.Arguments[2], scope), valueOperand);
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        if (!IsConstOrDefine(scope.Root, call.Arguments[0])) yield return new Error(call.Line, call.Column, "TypeHash argument should be constant", ErrorType.Semantic);
        //if (!IsConstOrDefine(scope.Root, call.Arguments[1])) yield return new Error(call.Line, call.Column, "NameHash argument should be constant", ErrorType.Semantic);
        if (!IsConstOrDefine(scope.Root, call.Arguments[2])) yield return new Error(call.Line, call.Column, "Variable argument should be constant", ErrorType.Semantic);
    }
}
