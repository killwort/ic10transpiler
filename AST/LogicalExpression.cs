using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal abstract class LogicalExpression : Expr
{
    internal LogicalExpression(Token srcToken) : base(srcToken)
    {
    }
}
