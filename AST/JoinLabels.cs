using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;

namespace Ic10Transpiler.AST;

internal class JoinLabels : Optimization
{
    public override bool Applicable(IEnumerable<Op> lines)
    {
        return (lines.First() is Label l1 && lines.Skip(1).First() is Label l2);
    }

    public override IEnumerable<Op> Optimize(IEnumerable<Op> lines)
    {
        if (lines.First() is Label l1 && lines.Skip(1).First() is Label l2)
        {
            l2.Name = l1.Name;
            yield return l1;
        }
    }
}