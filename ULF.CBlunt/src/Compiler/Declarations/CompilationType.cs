using System.Text;

namespace ULF.CBlunt.Compiler.Declarations;

public class CompilationType
{
	#pragma warning disable CS8625
	
	public static readonly CompilationType NoBase = new("", "", Type.Class, CompilationUnit.Empty, null, Array.Empty<CompilationType>(), false, 0, 0);
	
	#pragma warning restore CS8625

	public enum Type : byte
	{
		Struct,
		RefStruct,
		Class,
		RecordClass,
		ArrayType,
		Interface
	}

	public readonly string Namespace;
	public readonly string ClassName;
	public readonly CompilationUnit ParentUnit;
	public readonly bool IsReadonly;
	public readonly Type TypeOfDatatype;
	public readonly CompilationType ImmediateBase;
	public CompilationType[] ImplementedInterfaces;
	public readonly bool IsGeneric;
	public readonly byte NumTypeArgs;
	public CompilationType[] GenericArgs = Array.Empty<CompilationType>();
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
	public DtorDeclaration? DtorDecl;

	public CompilationType() { throw new Exception("no thanks"); }

	public CompilationType(string ns, string name, Type type, CompilationUnit parent, CompilationType immediateBase, CompilationType[] interfaces, bool isGeneric, byte numTypeArgs, ulong flags, ulong size = 0)
	{
		Namespace = ns;
		ClassName = name;
		ImmediateBase = immediateBase;
		ImplementedInterfaces = interfaces;
		IsGeneric = isGeneric;
		NumTypeArgs = numTypeArgs;
		IsReadonly = (flags & ModifierFlags.Readonly) != 0;
		TypeOfDatatype = type;
		ParentUnit = parent;
		Size = size;
		Flags = flags;
	}

	public CompilationType(string ns, string name, Type type, CompilationUnit parent, CompilationType immediateBase, CompilationType[] interfaces, ulong flags, ulong size = 0)
		: this(ns, name, type, parent, immediateBase, interfaces, false, 0, flags, size) { }

	public CompilationType(string ns, string name, Type type, CompilationUnit parent, CompilationType immediateBase, bool isGeneric, byte numTypeArgs, ulong flags, ulong size = 0)
		: this(ns, name, type, parent, immediateBase, Array.Empty<CompilationType>(), isGeneric, numTypeArgs, flags, size) { }

	public CompilationType(string ns, string name, Type type, CompilationUnit parent, CompilationType immediateBase, ulong flags, ulong size = 0)
		: this(ns, name, type, parent, immediateBase, Array.Empty<CompilationType>(), flags, size) { }

	public virtual StringBuilder ToFullyQualifiedString(bool forTypeDecl = false, bool withGenericArgs = false)
	{
		if (forTypeDecl && withGenericArgs) withGenericArgs = false;

		StringBuilder fullQual = new(Namespace.Length+2+ClassName.Length); 

		fullQual.Append('[');
		fullQual.Append(Namespace);
		fullQual.Append(']');
		fullQual.Append(ClassName);

		if (IsGeneric)
		{
			fullQual.Append('<');

			if (!withGenericArgs)
			{
				for (int i = 0; i < NumTypeArgs; i++)
				{
					fullQual.Append('T');
					fullQual.Append(i);
					if (i != NumTypeArgs-1) fullQual.Append(',');
				}
			}
			else
			{
				for (int i = 0; i < NumTypeArgs; i++)
				{
					fullQual.Append(GenericArgs[i].ToFullyQualifiedString(forTypeDecl));
					if (i != NumTypeArgs-1) fullQual.Append(',');
				}
			}

			fullQual.Append('>');
		}

		if (forTypeDecl)
		{
			fullQual.Append(':');
			if (ImmediateBase != NoBase) fullQual.Append(ImmediateBase.ToFullyQualifiedString(withGenericArgs: true));

			foreach (var intfc in ImplementedInterfaces)
			{
				fullQual.Append(',');
				fullQual.Append(intfc.ToFullyQualifiedString(withGenericArgs: true));
			}
		}

		return fullQual;
	}

	public StringBuilder ToNativeRepr()
	{
		StringBuilder builder = new(20);

		if (Namespace.Length > 0)
		{
			builder.Append("ns");
			builder.Append(Namespace.Count(c => c == '.')+1);
			builder.Append('_');
			builder.Append(Namespace.Replace('.', '_'));
		}
		else builder.Append("ns0");

		builder.Append('_');
		builder.Append(ClassName);

		return builder;
}

	public StringBuilder BuildMeta()
	{
		IEnumerable<IMemberDeclaration> decls = CtorDecls.Cast<IMemberDeclaration>() // well, there goes our memory & performance while putting all of these things into one list (even though it isn't necessary)
			.Concat(MethodDecls.Cast<IMemberDeclaration>())
			.Concat(FieldDecls.Cast<IMemberDeclaration>())
			.Concat(PropertyDecls.Cast<IMemberDeclaration>());

		if (DtorDecl.HasValue) decls = decls.Append(DtorDecl.Value);

		StringBuilder meta = new(300);

		meta.Append(IMemberDeclaration.GenerateModifierString(Flags));
		meta.Append(IsClass ? 'c' : 'v');
		meta.Append(ToFullyQualifiedString(forTypeDecl: true));
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

	public CompilationType ApplyTypeArgs(CompilationType[] genericTypeArgs)
	{
		return new(Namespace, ClassName, TypeOfDatatype, ParentUnit, ImmediateBase, ImplementedInterfaces, true, NumTypeArgs, Flags, Size)
		{
			GenericArgs = genericTypeArgs
		};
	}
}
