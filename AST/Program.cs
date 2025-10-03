using System;
using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.AST.Builtin;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class Program : IResolutionScope
{
    public Program Root => this;

    public Program(List<Define> defines, List<ISymbol> symtable, List<Statement> body)
    {
        Defines = defines;
        Symtable = symtable;
        Symtable.Add(new Sleep());
        Symtable.Add(new Status());
        Symtable.Add(new Load());
        Symtable.Add(new LoadByReference());
        Symtable.Add(new LoadBatch());
        Symtable.Add(new LoadBatchNamed());
        Symtable.Add(new LoadSlotBatch());
        Symtable.Add(new LoadSlotBatchNamed());
        Symtable.Add(new Save());
        Symtable.Add(new SaveBatch());
        Symtable.Add(new SaveBatchNamed());
        Symtable.Add(new SaveSlotBatch());
        Symtable.Add(new BasicArgless("rand", "rand"));
        Symtable.Add(new BasicUnary("abs", "abs"));
        Symtable.Add(new BasicUnary("ceil", "ceil"));
        Symtable.Add(new BasicUnary("exp", "exp"));
        Symtable.Add(new BasicUnary("floor", "floor"));
        Symtable.Add(new BasicUnary("log", "log"));
        Symtable.Add(new BasicUnary("round", "round"));
        Symtable.Add(new BasicUnary("sqrt", "sqrt"));
        Symtable.Add(new BasicUnary("trunc", "trunc"));
        Symtable.Add(new BasicUnary("sin", "sin"));
        Symtable.Add(new BasicUnary("cos", "cos"));
        Symtable.Add(new BasicUnary("asin", "asin"));
        Symtable.Add(new BasicUnary("acos", "acos"));
        Symtable.Add(new BasicUnary("atan", "atan"));
        Symtable.Add(new BasicUnary("tan", "tan"));
        Symtable.Add(new BasicBinary("min", "min"));
        Symtable.Add(new BasicBinary("max", "max"));

        Symtable.AddRange(body.OfType<FunctionDeclaration>());
        Symtable.AddRange(defines);
        Body = body;
    }

    public List<Define> Defines;
    public List<ISymbol> Symtable { get; }

    public ISymbol LookupSymbol(SymbolRef r)
    {
        return Symtable.FirstOrDefault(x => x.Name == r.Name);
    }

    public List<Statement> Body;
    public Stack<LoopStatement> LoopStack { get; private set; }

    private int labelId = 0;
    internal string UniqueLabel() => $"lbl_{labelId++}";

    public void Validate()
    {
        var errors = new List<Error>();
        LoopStack = new Stack<LoopStatement>();
        foreach (var s in Body)
        {
            if (s is FunctionDeclaration) continue;
            errors.AddRange(s.Validate(this).ToArray());
        }

        foreach (var fn in Symtable.OfType<FunctionDeclaration>())
        {
            fn.Root = this;
            errors.AddRange(fn.Validate(this));
        }

        var usedNames = new HashSet<string>();
        foreach (var sym in Symtable)
        {
            if (usedNames.Contains(sym.Name))
                errors.Add(new Error(sym.Line, sym.Column, $"Symbol {sym.Name} already declared", ErrorType.Semantic));
            usedNames.Add(sym.Name);
        }

        if (Symtable.OfType<VarDeclaration>().Count() >= 14)
            errors.Add(new Error(0, 0, "Program uses too much variables in global scope", ErrorType.Semantic));

        if (errors.Any()) throw new InvalidSyntaxException(errors.ToArray());
    }

    public string Emit()
    {
        var lines = new List<Op>();
        //Emit defines
        foreach (var def in Defines)
            lines.Add(def.Emit());
        //Assign main body variables to registers
        var reg = 15;
        LoopStack = new Stack<LoopStatement>();
        foreach (var sym in Symtable.OfType<IRegister>())
            sym.AssignedRegister = reg--;
        //Assign function variables to registers
        foreach (var fn in Symtable.OfType<FunctionDeclaration>())
        {
            reg = 15;
            fn.Root = this;
            foreach (var sym in fn.Symtable.OfType<IRegister>())
                sym.AssignedRegister = reg--;
            fn.ClaimLabels(this);
        }

        //Emit main body
        foreach (var s in Body.Where(x => !(x is FunctionDeclaration)))
            lines.AddRange(s.Emit(this));
        //Emit functions
        foreach (var fn in Symtable.OfType<FunctionDeclaration>())
            lines.AddRange(fn.Emit(fn));

        var linesBeforeOptimization = lines.Count;
        var optimizations = new Optimization[]
        {
            new JoinLabels(),
            new JoinMovesViaAcc(),
            new JoinMovePush(),
            new JoinCompareJump(),
            new RemoveUnreachable(),
            new RemoveJumpNext()
        };
        //Optimization
        for (var i = 0; i < lines.Count - 1; i++)
        {
            foreach (var optimization in optimizations)
            {
                if (optimization.Lookahead < lines.Count - i && optimization.Applicable(lines.Skip(i).Take(optimization.Lookahead)))
                {
                    var optimized = optimization.Optimize(lines.Skip(i).Take(optimization.Lookahead)).ToArray();
                    lines.RemoveRange(i, optimization.Lookahead);
                    lines.InsertRange(i, optimized);
                    i -= optimization.Lookahead - optimized.Length;
                }
            }
        }

        var linesAfterOptimization = lines.Count;
        // Console.Error.WriteLine($"{linesBeforeOptimization} lines reduced to {linesAfterOptimization}. {100 * (linesBeforeOptimization - linesAfterOptimization) / linesBeforeOptimization}% decrease");
        return string.Join('\n', lines);
    }
}
