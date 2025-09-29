using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class BasicUnary : BuiltinFunction
{
    private readonly string _opcode;

    private static Argument[] arguments = new[]
    {
        new Argument(new Token(), "arg")
    };

    public BasicUnary(string name, string opcode)
    {
        _opcode = opcode;
        Name = name;
    }

    public override string Name { get; }
    public override IEnumerable<Argument> Arguments => arguments;

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        Operand valueOperand = Acc0Operand.Instance;
        if (IsConstOrDefine(scope.Root, call.Arguments[0]))
            valueOperand = ToOperand(call.Arguments[0]);
        else
            foreach (var l in call.Arguments[0].Emit(scope))
                yield return l;
        yield return new Instruction(call, _opcode, Acc0Operand.Instance, valueOperand);
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        yield break;
    }
}