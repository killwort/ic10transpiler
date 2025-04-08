using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class Block : Statement
{
    internal Block(Token srcToken, IEnumerable<Statement> body) : base(srcToken)
    {
        Body = body.ToList();
    }

    internal readonly List<Statement> Body = new List<Statement>();

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        foreach (var s in Body)
        {
            foreach (var e in s.Validate(scope)) yield return e;
        }
    }

    public override bool IsAllBranchesHasReturn()
    {
        bool hasReturn = false;
        foreach (var s in Body)
        {
            if (!hasReturn)
            {
                if (s is Return) hasReturn = true;
                else if (s.IsAllBranchesHasReturn()) hasReturn = true;
            }
        }

        return hasReturn;
    }

    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        return Body.SelectMany(x => x.Emit(scope)).ToArray();
    }
}
