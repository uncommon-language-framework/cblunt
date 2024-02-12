namespace ULF.CBlunt.Compiler;


public class CompilationUnit
{
	public static readonly CompilationUnit Empty = new(string.Empty, string.Empty);
	
	public readonly List<CompilationType> Types = new();
	public readonly string AssemblyName;
	public readonly string Source;
	public string Metadata = string.Empty;
	
	public CompilationUnit(string assemblyName, string sourceCode)
	{
		AssemblyName = assemblyName;
		Source = sourceCode;
	}
}