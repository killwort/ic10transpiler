using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class Argument : Symbol, IRegister
{
    internal Argument(Token srcToken, string name) : base(srcToken, name)
    {
    }
    
    public int AssignedRegister { get; set; }
}
