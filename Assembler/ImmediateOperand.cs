using System.Globalization;

namespace Ic10Transpiler.Assembler;

internal class ImmediateOperand : Operand
{
    public ImmediateOperand(float value)
    {
        Value = value;
    }

    public float Value;
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
