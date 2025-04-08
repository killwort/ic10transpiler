using System;
using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class BinaryExpression : Expr
{
    internal BinaryExpression(Token srcToken, Expr left, string @operator, Expr right) : base(srcToken)
    {
        Left = left;
        Right = right;
        Operator = @operator;
    }

    internal Expr Left, Right;
    internal string Operator;

    public override IEnumerable<Error> Validate(IResolutionScope scope)
    {
        foreach (var e in Left.Validate(scope)) yield return e;
        foreach (var e in Right.Validate(scope)) yield return e;
    }


    public override IEnumerable<Op> Emit(IResolutionScope scope)
    {
        foreach (var l in Left.Emit(scope)) yield return l;
        yield return new Instruction(this,"push", Acc0Operand.Instance); 
        foreach (var l in Right.Emit(scope)) yield return l;
        yield return new Instruction(this,"pop", Acc1Operand.Instance); 
        var opcode = Operator switch
        {
            "+" => "add",
            "-" => "sub",
            "*" => "mul",
            "/" => "div",
            "%" => "mod",
            _ => throw new InvalidOperationException()
        };
        yield return new Instruction(this, opcode, Acc0Operand.Instance, Acc1Operand.Instance, Acc0Operand.Instance); 
    }
}
