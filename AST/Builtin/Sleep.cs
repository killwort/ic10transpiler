using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST.Builtin;

internal class Sleep : BuiltinFunction
{
    public override string Name => "sleep";

    public override IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call)
    {
        yield return new Instruction(call, "yield");
    }

    public override IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call)
    {
        yield break;
    }
}