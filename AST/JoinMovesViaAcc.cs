using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;

namespace Ic10Transpiler.AST;

internal class JoinMovesViaAcc : Optimization
{
    public override bool Applicable(IEnumerable<Op> lines)
    {
        return (lines.First() is Instruction i1 && lines.Skip(1).First() is Instruction i2 && i1.OpCode == "move" && i2.OpCode == "move" &&
                i1.Operands[0].Equals(i2.Operands[1]));
        //((i1.Operands[0] == Acc0Operand.Instance && i2.Operands[1] == Acc0Operand.Instance) || (i1.Operands[0] == Acc1Operand.Instance && i2.Operands[1] == Acc1Operand.Instance)));
    }

    public override IEnumerable<Op> Optimize(IEnumerable<Op> lines)
    {
        //Simplify move r0 x; move rN r0 constructions
        var i1 = (Instruction)lines.First();
        var i2 = (Instruction)lines.Skip(1).First();
        i1.Operands[0] = i2.Operands[0];
        yield return i1;
    }
}
