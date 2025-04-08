using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class VarDeclaration : Symbol, IRegister
{
    internal VarDeclaration(Token srcToken, string name) : base(srcToken, name)
    {
    }

    public int AssignedRegister { get; set; }
}
