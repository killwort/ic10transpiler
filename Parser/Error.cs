namespace Ic10Transpiler.Parser;

/// <summary>
///     Source code error.
/// </summary>
internal class Error
{
    internal Error(int line, int column, string message, ErrorType type)
    {
        Line = line;
        Column = column;
        Message = message;
        Type = type;
    }

    /// <summary>
    ///     Source line number.
    /// </summary>
    internal int Line { get; }

    /// <summary>
    ///     Source column number.
    /// </summary>
    internal int Column { get; }

    /// <summary>
    ///     Error message.
    /// </summary>
    internal string Message { get; }

    /// <summary>
    ///     Type of error.
    /// </summary>
    internal ErrorType Type { get; }
}
