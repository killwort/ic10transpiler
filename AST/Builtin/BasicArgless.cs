using System;
using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class BasicArgless : BuiltinFunction
{
    private readonly string _opcode;

    public BasicArgless(string name, string opcode)
    {
        _opcode = opcode;
        Name = name;
    }

    public override string Name { get; }
    public override IEnumerable<Argument> Arguments => Array.Empty<Argument>();

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        yield return new Instruction(call, _opcode, Acc0Operand.Instance);
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        yield break;
    }
}