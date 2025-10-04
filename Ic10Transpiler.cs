using System;
using System.Collections.Generic;
using System.IO;
using Ic10Transpiler.AST;
using Ic10Transpiler.Parser;

namespace Ic10Transpiler;

public static class Ic10Transpiler
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
            ProcessSource(Console.In.ReadToEnd(), new List<Define>());
        else
        {
            var defs = new List<Define>();
            var files = new List<string>();
            foreach (var arg in args)
            {
                if (arg.StartsWith("-D"))
                {
                    var parts = arg.Substring(2).Split('=', 2);
                    defs.Add(new Define(new Token(), parts[0]) { Value = parts.Length > 1 ? parts[1] : "1" });
                }
                else files.Add(arg);
            }
            foreach (var src in files)
                ProcessSource(File.ReadAllText(src), defs);
        }
    }

    private static void ProcessSource(string code, List<Define> defs)
    {
        var codeLines = code.Split("\n");
        try
        {
            var program = Parser.Parser.Parse(code);
            program.Defines.AddRange(defs);
            program.Symtable.AddRange(defs);
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
