using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class BasicBinary : BuiltinFunction
{
    private readonly string _opcode;

    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "arg1"),
        new Argument(new Token(), "arg2")
    };

    public BasicBinary(string name, string opcode)
    {
        _opcode = opcode;
        Name = name;
    }

    public override string Name { get; }
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        foreach (var l in call.Arguments[1].Emit(scope)) yield return l;
        yield return new Instruction(call, "push", Acc0Operand.Instance);
        Operand valueOperand = Acc0Operand.Instance;
        if (IsConstOrDefine(scope.Root, call.Arguments[0]))
            valueOperand = ToOperand(call.Arguments[0], scope);
        else
            foreach (var l in call.Arguments[0].Emit(scope))
                yield return l;
        yield return new Instruction(call, "pop", Acc1Operand.Instance);
        yield return new Instruction(call, _opcode, Acc0Operand.Instance, valueOperand, Acc1Operand.Instance);
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        yield break;
    }
}
