using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ULF.CBlunt.Compiler.Declarations;
using ULF.CBlunt.Compiler.DefaultTypes;

namespace ULF.CBlunt.Compiler;

class CBNativeSyntaxWalker : CSharpSyntaxWalker
{
	static readonly SymbolDisplayFormat ResolveFullyQualifiedName = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
	CompilationUnit unit = CompilationUnit.Empty;
	readonly Stack<CompilationType> currentTypeStack = new(3);
	string currentMethodBody = string.Empty;
	
	#pragma warning disable CS8618
	SemanticModel model;
	CompilationType[] referencedTypes;
	#pragma warning restore CS8618

	public void Init(CompilationUnit compilationUnit, SyntaxTree tree, CompilationType[] referencedTypes)
	{
		unit = compilationUnit;
		model = CSharpCompilation
			.Create(unit.AssemblyName, new[] { tree })
			.GetSemanticModel(tree);
		this.referencedTypes = referencedTypes;
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

	static string FindNamespaceOf(SyntaxNode node)
	{
		NamespaceDeclarationSyntax? nsNode = node.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

		if (nsNode is null) return "";

		return nsNode.Name.ToString();
	}

	CompilationType FindType(TypeSyntax type)
	{
		return FindType(
			model.GetTypeInfo(type).Type ?? throw new Exception("??? why null")
		);
	}

	CompilationType FindType(ITypeSymbol typeSym)
	{
		if (typeSym is IArrayTypeSymbol symAsArrayType)
		{
			FindType(symAsArrayType.ElementType); // ensure element type exists

			// this type doesn't have to be part of an assembly, we just want to know its name for the metadata, the ULR generates array type pointers on the fly upon loading
			return new CompilationType("", symAsArrayType.ToDisplayString(ResolveFullyQualifiedName), CompilationType.Type.ArrayType, CompilationUnit.Empty, CompilationType.NoBase, Array.Empty<CompilationType>(), false, 0, ModifierFlags.Public | ModifierFlags.Sealed);
		}

		string fullyQualifiedName = typeSym.ToDisplayString(ResolveFullyQualifiedName);

		foreach (var compType in unit.Types)
		{
			// we morph the possible target type because we don't make a difference between ns and nested type
			if (compType.ToFullyQualifiedString().Replace("[", "").Replace(']', '.').ToString() == fullyQualifiedName) return compType;
		}

		foreach (var compType in referencedTypes)
		{
			if (compType.ToFullyQualifiedString().Replace("[", "").Replace(']', '.').ToString() == fullyQualifiedName) return compType;
		}


		throw new Exception($"Type {fullyQualifiedName} not found");
		// TODO: Have to be able to find type by name (even when they are ambiguous due to a using directive)
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

		CompilationType type = new(ns, name, declType, unit, CompilationType.NoBase, Array.Empty<CompilationType>(), false, 0, modifiers, declType == CompilationType.Type.Class || declType == CompilationType.Type.Interface ? DefaultMetadata.DEFAULT_REF_OBJ_SIZE : 0);

		unit.Types.Add(type);
		currentTypeStack.Push(type);
	}

	public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
	{
		base.VisitMethodDeclaration(node);

		string name = node.Identifier.ValueText;

		CompilationType currentType = currentTypeStack.Peek();

		var paramList = node.ParameterList.Parameters;

		CompilationType[] paramTypes = new CompilationType[paramList.Count];
		string[] paramNames = new string[paramList.Count];

		for (int i = 0; i < paramList.Count; i++)
		{
			TypeSyntax paramType = paramList[i].Type ?? throw new Exception($"why null paramsyntax -> {paramList[i].ToFullString()}");
			
			paramTypes[i] = FindType(paramType);
			paramNames[i] = paramList[i].Identifier.ValueText;
		}

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
				case "virtual":
					modifiers |= ModifierFlags.Virtual;
					break;
				case "extern":
					modifiers |= ModifierFlags.Extern;
					break;
			}
		}
		
		currentType.MethodDecls.Add(
			new MethodDeclaration(
				name,
				currentMethodBody,
				currentType,
				paramTypes,
				paramNames,
				FindType(node.ReturnType),
				modifiers
			)
		);

		currentMethodBody = string.Empty;
	}

	public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
	{
		base.VisitConstructorDeclaration(node);
	}

	public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
	{
		base.VisitDestructorDeclaration(node);
	}
}