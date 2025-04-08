using System.Collections.Generic;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal abstract class Expr : Statement
{
    internal Expr(Token srcToken) : base(srcToken)
    {
    }
    public override bool IsAllBranchesHasReturn() => false;
}
