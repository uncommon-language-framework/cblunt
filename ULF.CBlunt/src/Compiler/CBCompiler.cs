using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ULF.CBlunt.Compiler;


class CBCompiler
{
	public readonly string Source;
	public readonly SyntaxTree SyntaxTree;
	public CompilationUnit CompilationUnit;

	public CBCompiler(string code, SyntaxTree syntaxTree, string assemblyName)
	{
		Source = code;
		SyntaxTree = syntaxTree;
		CompilationUnit = new(assemblyName, code);
	}

	public string BuildMeta()
	{
		StringBuilder meta = new(10000);

		foreach (var type in CompilationUnit.Types)
		{
			meta.Append(type.BuildMeta());
			meta.AppendLine();
		}

		return meta.ToString();
	}

	public CompilationUnit Compile()
	{
		var root = SyntaxTree.GetCompilationUnitRoot();

		CSNativeSyntaxWalker walker = new();

		walker.Init(CompilationUnit);
		walker.Visit(root);

		CompilationUnit.Metadata = BuildMeta();

		return CompilationUnit;
	}
}