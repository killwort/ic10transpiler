namespace Ic10Transpiler.Assembler;

internal class DeviceOperand : Operand
{
    public DeviceOperand(string device)
    {
        Device = device;
    }

    public string Device;
    public override string ToString() => Device;
    
    public override bool Equals(Operand other)
    {
        return other is DeviceOperand op &&  op.Device == Device;
    }
}
