namespace Ic10Transpiler.Parser;

internal interface IErrors
{
    void SemErr(int line, int col, string s);
    void SemErr(string s);
    void SynErr(int line, int col, int n);
    void Warning(int line, int col, string s);
    void Warning(string s);
}