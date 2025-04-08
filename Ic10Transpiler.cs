using System;
using System.IO;

namespace Ic10Transpiler;

public static class Ic10Transpiler
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
            ProcessSource(Console.In.ReadToEnd());
        else
            foreach (var src in args)
                ProcessSource(File.ReadAllText(src));
    }

    private static void ProcessSource(string code)
    {
        var codeLines = code.Split("\n");
        try
        {
            var program = Parser.Parser.Parse(code);
            program.Validate();
            Console.WriteLine(program.Emit());
        }
        catch (InvalidSyntaxException e)
        {
            foreach (var err in e.Errors)
            {
                Console.Error.WriteLine($"{err.Line}:{err.Column}: [{err.Type}] {err.Message}");
                if (err.Line > 0)
                    Console.Error.WriteLine(codeLines[err.Line - 1]);
                if (err.Column > 0)
                {
                    Console.Error.Write(new string(' ', err.Column - 1));
                    Console.Error.WriteLine("^");
                }
            }
        }
    }
}
