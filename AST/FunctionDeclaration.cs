using System;
using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class FunctionDeclaration : Statement, ISymbol, IResolutionScope, IFunction
{
    internal FunctionDeclaration(Token srcToken, string name) : base(srcToken)
    {
        Name = name;
    }

    public string Name { get; }
    public List<ISymbol> Symtable { get; } = new List<ISymbol>();
    public IEnumerable<Argument> Arguments => Symtable.OfType<Argument>();
    public Stack<LoopStatement> LoopStack { get; }
    internal readonly List<Statement> Body = new List<Statement>();
    internal Label EntryLabel;
    internal Label ExitLabel;
    public Program Root { get; set; }

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        var hasReturn = false;
        foreach (var s in Body)
        {
            foreach (var e in s.Validate(this)) yield return e;
            if (!hasReturn)
            {
                if (s is Return) hasReturn = true;
                else if (s.IsAllBranchesHasReturn()) hasReturn = true;
            }
        }

        if (Symtable.Count > 14)
            yield return new Error(Line, Column, $"Function {Name} uses too much variables+arguments scope", ErrorType.Semantic);
        var usedNames = new HashSet<string>(scope.Root.Defines.Select(x => x.Name));
        foreach (var sym in Symtable)
        {
            if(usedNames.Contains(sym.Name))
                yield return new Error(sym.Line, sym.Column, $"Symbol {sym.Name} already declared", ErrorType.Semantic);
            usedNames.Add(sym.Name);
        }
        if (!hasReturn)
            yield return new Error(Line, Column, $"Function {Name} should have return in all execution branches", ErrorType.Semantic);
    }

    public override bool IsAllBranchesHasReturn()
    {
        //We shouldn't get here, because functions cannot contain functions.
        throw new NotSupportedException();
    }

    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        yield return EntryLabel;
        foreach (var arg in Symtable.OfType<Argument>().Reverse())
            yield return new Instruction(this, "pop", new RegOperand(arg));
        yield return new Instruction(this, "push", new RegReturnAddressOperand());
        foreach (var s in Body)
        foreach (var l in s.Emit(this))
            yield return l;
        yield return ExitLabel;
        yield return new Instruction(this, "pop", new RegReturnAddressOperand());
        yield return new Instruction(this, "j", new RegReturnAddressOperand());
    }

    public void ClaimLabels(IResolutionScope scope)
    {
        EntryLabel = new Label(this, scope.Root.UniqueLabel() + "_fn_" + Name + "_entry");
        ExitLabel = new Label(this, scope.Root.UniqueLabel() + "_fn_" + Name + "_exit");
    }
}
