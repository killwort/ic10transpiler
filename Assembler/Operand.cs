using System;

namespace Ic10Transpiler.Assembler;

internal abstract class Operand:IEquatable<Operand>
{
    public abstract bool Equals(Operand other);
}
