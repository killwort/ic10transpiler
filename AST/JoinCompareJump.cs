using System.Collections.Generic;
using System.Linq;
using Ic10Transpiler.Assembler;

namespace Ic10Transpiler.AST;

internal class JoinCompareJump : Optimization
{
    private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
    {
        { "seq", "bne" },
        { "sgt", "ble" },
        { "sge", "blt" },
        { "slt", "bge" },
        { "sle", "bgt" },
        { "sne", "beq" },
    };
    public override bool Applicable(IEnumerable<Op> lines)
    {
        //Simplify seq r0 x y; breqz r0 z constructions
        return (lines.First() is Instruction i11 && lines.Skip(1).First() is Instruction i12 && Map.ContainsKey(i11.OpCode) && i12.OpCode == "beqz" &&
                ((i11.Operands[0] == Acc0Operand.Instance && i12.Operands[0] == Acc0Operand.Instance) || (i11.Operands[0] == Acc1Operand.Instance && i12.Operands[0] == Acc1Operand.Instance)));
    }

    public override IEnumerable<Op> Optimize(IEnumerable<Op> lines)
    {
        var cmp = (Instruction)lines.First();
        var jmp = (Instruction)lines.Skip(1).First();
        yield return new Instruction(cmp.Source, Map[cmp.OpCode], cmp.Operands[1], cmp.Operands[2], jmp.Operands[1]);
    }
}
