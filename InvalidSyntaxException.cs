using System;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler;

/// <summary>
///     Ошибка разбора выражения.
/// </summary>
internal class InvalidSyntaxException : Exception
{
    internal InvalidSyntaxException(Error[] errors) => Errors = errors;

    /// <summary>
    ///     Список ошибок.
    /// </summary>
    internal Error[] Errors { get; }
}