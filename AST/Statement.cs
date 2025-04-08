using System.Collections.Generic;
using Ic10Transpiler.Assembler;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal abstract class Statement : SourceMappedElement
{
    internal Statement(Token srcToken) : base(srcToken)
    {
    }

    public abstract IEnumerable<Error> Validate(IResolutionScope scope);
    public abstract bool IsAllBranchesHasReturn();

    public abstract IEnumerable<Op> Emit(IResolutionScope scope);
}
