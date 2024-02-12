using System.Text;
using ULF.CBlunt.Compiler.Declarations;

namespace ULF.CBlunt.Compiler;

public class CompilationType
{
	public enum Type : byte
	{
		Struct,
		RefStruct,
		Class,
		RecordClass
	}

	public readonly string Namespace;
	public readonly string ClassName;
	public readonly CompilationUnit ParentUnit;
	public readonly bool IsReadonly;
	public readonly Type TypeOfDatatype;
	public readonly ulong Flags;
	public ulong Size;


	public bool IsStruct => TypeOfDatatype == Type.Struct;
	public bool IsClass => TypeOfDatatype == Type.Class;
	public bool IsRefStruct => TypeOfDatatype == Type.RefStruct;
	public bool IsRecord => TypeOfDatatype == Type.RecordClass;
	public bool IsReadonlyStruct => IsStruct && IsReadonly;
	public bool IsReadonlyRefStruct => IsRefStruct && IsReadonly;

	public readonly List<CtorDeclaration> CtorDecls = new();
	public readonly List<MethodDeclaration> MethodDecls = new();
	public readonly List<FieldDeclaration> FieldDecls = new();
	public readonly List<PropertyDeclaration> PropertyDecls = new();
	public DtorDeclaration DtorDecl;


	public CompilationType(string ns, string name, bool isReadonly, Type type, CompilationUnit parent, ulong flags, ulong size = 0)
	{
		Namespace = ns;
		ClassName = name;
		IsReadonly = isReadonly;
		TypeOfDatatype = type;
		ParentUnit = parent;
		Size = size;
		Flags = flags;
	}

	public StringBuilder ToFullyQualifiedString()
	{
		StringBuilder fullQual = new(Namespace.Length+2+ClassName.Length);

		fullQual.Append('[');
		fullQual.Append(Namespace);
		fullQual.Append(']');
		fullQual.Append(ClassName);

		return fullQual;
	}

	public StringBuilder BuildMeta()
	{
		IEnumerable<IMemberDeclaration> decls = CtorDecls.Cast<IMemberDeclaration>() // well, there goes our memory & performance while putting all of these things into one list (even though it isn't necessary)
			.Append(DtorDecl)
			.Concat(MethodDecls.Cast<IMemberDeclaration>())
			.Concat(FieldDecls.Cast<IMemberDeclaration>())
			.Concat(PropertyDecls.Cast<IMemberDeclaration>());

		StringBuilder meta = new(300);

		meta.Append(IMemberDeclaration.GenerateModifierString(Flags));
		meta.Append(IsClass ? 'c' : 'v');
		meta.Append(ToFullyQualifiedString());
		meta.Append('$');
		meta.Append(Size);
		meta.Append(';');


		foreach (var decl in decls)
		{
			meta.Append(decl.BuildMeta());
			meta.Append(';');
		}

		return meta;
	}
}
