using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;

namespace Ic10Transpiler.AST;

internal class RemoveJumpNext : Optimization
{
    public override bool Applicable(IEnumerable<Op> lines)
    {
        //Remove unconditional jump to next line
        return (lines.First() is Instruction i31 && lines.Skip(1).First() is Label i32 && i31.OpCode == "j" && i31.Operands[0] is LabelOperand lo31 && lo31.Target.Name == i32.Name);
    }

    public override IEnumerable<Op> Optimize(IEnumerable<Op> lines)
    {
        yield return lines.Skip(1).First();
    }
}