using Ic10Transpiler.Parser;

namespace Ic10Transpiler.AST;

internal class StringBuffer : IBuffer
{
    internal const int EOF = char.MaxValue + 1;
    private readonly string _src;

    internal StringBuffer(string src) => _src = src;

    public int Read()
    {
        Pos++;
        if (Pos >= _src.Length)
            return EOF;
        return _src[Pos];
    }

    public int Peek()
    {
        if (Pos + 1 >= _src.Length)
            return EOF;
        return _src[Pos + 1];
    }

    public int Pos { get; set; } = -1;

    public string GetString(int beg, int end) => _src.Substring(beg, end - beg);
}