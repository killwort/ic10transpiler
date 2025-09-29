using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal abstract class BuiltinFunction : IFunction
{
    public int Line => -1;
    public int Column => -1;
    public virtual string Name { get; }
    public virtual IEnumerable<Argument> Arguments { get; }
    public abstract IEnumerable<Op> EmitCall(IResolutionScope scope, FunctionCall call);
    public abstract IEnumerable<Error> ValidateCall(IResolutionScope scope, FunctionCall call);

    protected static bool IsConstOrDefine(Program program, Expr arg)
    {
        if (arg is ConstValue) return true;
        if (arg is SymbolRef sr && program.LookupSymbol(sr) is Define) return true;
        return false;
    }

    protected static Operand ToOperand(Expr arg, IResolutionScope scope)
    {
        if (arg is ConstValue cv) return new ImmediateAnyOperand(cv.Value);
        if (arg is SymbolRef sr)
        {
            var emit=arg.Emit(scope);
            return ((Instruction)emit.First()).Operands.Last();
            //return new DefinitionOperand(sr.Name);
        }
        throw new InvalidOperationException();
    }
}
