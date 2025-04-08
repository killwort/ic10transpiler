using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class Define : Symbol
{
    internal string Value;

    internal Define(Token srcToken, string name) : base(srcToken, name)
    {
    }

    public Op Emit()
    {
        return new DefineOp(this, Name, Value);
    }
}
