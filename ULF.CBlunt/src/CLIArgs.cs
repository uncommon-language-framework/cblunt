using System.Collections;
using CommandLine;

namespace ULF.CBlunt;

class CLIArgs
{
	[Value(0, MetaName = "file", HelpText = "The files to compile.", Required = true)]
	public IEnumerable<string> FileNames { get; set; } = Array.Empty<string>();

	[Option('o', "out", HelpText = "Output filename.", Required = false, Default = null)]
	public string OutFile { get; set; } = string.Empty;
	public string CompilationAssemblyName = string.Empty;
	public static CLIArgs ParseArgs(string[] args)
	{
		var result = CommandLine.Parser.Default.ParseArguments<CLIArgs>(args);

		if (result.Errors.Any()) Environment.Exit(1);
		
		CLIArgs parsedArgs = result.Value;
	
		parsedArgs.OutFile = parsedArgs.OutFile == string.Empty ? "OutputDecompiledAssembly.cpp" : parsedArgs.OutFile;

		parsedArgs.CompilationAssemblyName = parsedArgs.OutFile[..parsedArgs.OutFile.LastIndexOf('.')]+".dll";

		foreach (var fileName in parsedArgs.FileNames)
		{
			if (!File.Exists(fileName))
			{
				Console.Error.WriteLine($"C$ Compiler Error: input file '{fileName}' does not exist!");
				Environment.Exit(1);
			}
		}

		return parsedArgs;
	}
}