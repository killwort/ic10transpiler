using System.Collections.Generic;
using Ic10Transpiler.Assembler;

namespace Ic10Transpiler.AST;

internal abstract class Optimization
{
    public virtual int Lookahead => 2;
    public abstract bool Applicable(IEnumerable<Op> lines);
    public abstract IEnumerable<Op> Optimize(IEnumerable<Op> lines);
}