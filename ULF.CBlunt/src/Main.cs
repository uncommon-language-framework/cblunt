using ULF.CBlunt.Compiler;
using ULF.CBlunt.Parser;
using Microsoft.CodeAnalysis;

namespace ULF.CBlunt;

class Program
{
	static int Main(string[] args)
	{
		CLIArgs parsedArgs = CLIArgs.ParseArgs(args);

		foreach (var fileName in parsedArgs.FileNames)
		{
			string code;

			try { code = File.ReadAllText(fileName); }
			catch (Exception e)
			{
				Console.Error.WriteLine($"C$ Compiler Error: could not read input file '{fileName}' -- ${e.Message}");
				return 1;
			}

			SyntaxTree syntaxTree;
			List<string> infos;
			List<string> warnings;
			List<string> errors;

			(syntaxTree, infos, warnings, errors) = WrappedParser.ParseCode(code);
			
			foreach (var msg in infos)
			{
				Console.WriteLine($"{fileName}: INFO: {msg}");
			}

			foreach (var msg in warnings)
			{
				Console.WriteLine($"{fileName}: WARNING: {msg}");
			}

			foreach (var msg in errors)
			{
				Console.WriteLine($"{fileName}: ERROR: {msg}");
			}

			if (errors.Any()) return 1;

			var compiler = new CBCompiler(code, syntaxTree, parsedArgs.CompilationAssemblyName);

			CompilationUnit result = compiler.Compile();
			
			Console.WriteLine($"// Output Decompiled Assembly {result.AssemblyName} (source file {fileName}):");

			Console.WriteLine();
			Console.WriteLine(result.ToNativeCode());
		}

		return 0;
	}
}
