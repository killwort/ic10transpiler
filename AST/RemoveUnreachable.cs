using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;

namespace Ic10Transpiler.AST;

internal class RemoveUnreachable : Optimization
{
    public override bool Applicable(IEnumerable<Op> lines)
    {
//Remove all instructions between unconditional jump and label
        return (lines.First() is Instruction i21 && lines.Skip(1).First() is Instruction && i21.OpCode == "j");
    }

    public override IEnumerable<Op> Optimize(IEnumerable<Op> lines)
    {
        yield return lines.Skip(1).First();
    }
}