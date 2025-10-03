using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;

namespace Ic10Transpiler.AST;

internal class JoinMovePush : Optimization
{
    public override bool Applicable(IEnumerable<Op> lines)
    {
//Simplify move r0 x; push r0 constructions
        return (lines.First() is Instruction i1 && lines.Skip(1).First() is Instruction i2 && i1.OpCode == "move" && i2.OpCode == "push" &&
                i1.Operands[0].Equals(i2.Operands[0])
            /*((i1.Operands[0] == Acc0Operand.Instance && i2.Operands[0] == Acc0Operand.Instance) || (i1.Operands[0] == Acc1Operand.Instance && i2.Operands[0] == Acc1Operand.Instance))*/);
    }

    public override IEnumerable<Op> Optimize(IEnumerable<Op> lines)
    {
        var i1 = (Instruction)lines.First();
        var i2 = (Instruction)lines.Skip(1).First();

        i2.Operands[0] = i1.Operands[1];
        yield return i2;
    }
}