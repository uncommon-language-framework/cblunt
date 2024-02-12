using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ULF.CBlunt.Compiler.Declarations;

namespace ULF.CBlunt.Compiler;

class CSNativeSyntaxWalker : CSharpSyntaxWalker
{
	CompilationUnit unit = CompilationUnit.Empty;
	readonly Stack<CompilationType> currentType = new(3);

	public void Init(CompilationUnit compilationUnit)
	{
		unit = compilationUnit;
	}

	public override void VisitClassDeclaration(ClassDeclarationSyntax node)
	{
		VisitTypeDeclaration(node);
		base.VisitClassDeclaration(node);
	}

	public override void VisitStructDeclaration(StructDeclarationSyntax node)
	{
		VisitTypeDeclaration(node);
		base.VisitStructDeclaration(node);
	}

	public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
	{
		VisitTypeDeclaration(node);
		base.VisitInterfaceDeclaration(node);
	}

	public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
	{
		VisitTypeDeclaration(node);
		base.VisitRecordDeclaration(node);
	}

	string FindNamespaceOf(SyntaxNode node)
	{
		NamespaceDeclarationSyntax? nsNode = node.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

		if (nsNode is null) return "";

		return nsNode.Name.ToString();
	}

	public void VisitTypeDeclaration(TypeDeclarationSyntax node)
	{
		string ns = FindNamespaceOf(node);
		string name = node.Identifier.ValueText;
		CompilationType.Type declType = CompilationType.Type.Class;

		ulong modifiers = 0;

		foreach (var modToken in node.Modifiers)
		{
			switch (modToken.Text)
			{
				case "public":
					modifiers |= ModifierFlags.Public;
					break;
				case "protected":
					modifiers |= ModifierFlags.Protected;
					break;
				case "internal":
					modifiers |= ModifierFlags.Internal;
					break;
				case "static":
					modifiers |= ModifierFlags.Static;
					break;
				case "readonly":
					modifiers |= ModifierFlags.Readonly;
					break;
				case "abstract":
					modifiers |= ModifierFlags.Abstract;
					break;
				case "partial":
					modifiers |= ModifierFlags.Partial;
					break;
				case "sealed":
					modifiers |= ModifierFlags.Sealed;
					break;
			}
		}

		switch (node.Keyword.Text)
		{
			case "class": break;
			case "struct":
				declType = CompilationType.Type.Struct;
				break;
			case "record":
				declType = CompilationType.Type.RecordClass;
				break;
			case "ref struct":
				declType = CompilationType.Type.RefStruct;
				break;
		}

		CompilationType type = new(ns, name, (modifiers & ModifierFlags.Readonly) != 0, declType, unit, modifiers);

		unit.Types.Add(type);
		currentType.Push(type);
	}
}