using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class FunctionCall : Expr
{
    internal bool DiscardReturnValue { get; }

    internal FunctionCall(Token srcToken, string name, List<Expr> arguments, bool discardRetVal = false) : base(srcToken)
    {
        DiscardReturnValue = discardRetVal;
        Name = new SymbolRef(srcToken, name);
        Arguments = arguments;
    }

    internal SymbolRef Name;
    internal List<Expr> Arguments;

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        var targetFn = scope.Root.LookupSymbol(Name) as IFunction;
        if (targetFn == null)
            yield return new Error(Line, Column, $"Call of undefined function {Name}", ErrorType.Semantic);
        else if (targetFn.Arguments.Count() != Arguments.Count)
            yield return new Error(Line, Column, $"Incorrect number of arguments passed ({Arguments.Count}) to function {targetFn.Name} with {targetFn.Arguments.Count()} arguments", ErrorType.Semantic);
        if (targetFn is BuiltinFunction targetBFn)
            foreach (var l in targetBFn.ValidateCall(scope, this))
                yield return l;
    }

    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        var targetIFn = scope.Root.LookupSymbol(Name) as IFunction;
        if (targetIFn is FunctionDeclaration targetFn)
        {
            var regVars = scope.Symtable.OfType<IRegister>().ToArray();
            foreach (var reg in regVars)
                yield return new Instruction(this, "push", new RegOperand(reg));
            foreach (var arg in Arguments)
            {
                foreach (var l in arg.Emit(scope)) yield return l;
                yield return new Instruction(this, "push", Acc0Operand.Instance);
            }

            yield return new Instruction(this, "jal", new LabelOperand(targetFn.EntryLabel));
            foreach (var reg in regVars.Reverse())
                yield return new Instruction(this, "pop", new RegOperand(reg));
        }
        else if (targetIFn is BuiltinFunction targetBFn)
            foreach (var l in targetBFn.EmitCall(scope, this))
                yield return l;
    }
}
