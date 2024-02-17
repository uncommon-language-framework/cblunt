using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ULF.CBlunt.Compiler.Declarations;
using ULF.CBlunt.Compiler.DefaultTypes;

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

		CBNativeSyntaxWalker walker = new();

		walker.Init(CompilationUnit, SyntaxTree, DefaultMetadata.DefaultIncludedTypes);
		walker.Visit(root);

		// ULR will generate default ctors and dtors for us @ runtime
		CompilationUnit.Metadata = BuildMeta().ReplaceLineEndings("\n");

		return CompilationUnit;
	}
}