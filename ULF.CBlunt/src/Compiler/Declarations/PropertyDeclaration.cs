using System.Text;

namespace ULF.CBlunt.Compiler.Declarations;

public readonly struct PropertyDeclaration : IMemberDeclaration
{
	public readonly string Name;
	public readonly MethodDeclaration? Getter;
	public readonly MethodDeclaration? Setter;
	public readonly CompilationType For;
	public readonly CompilationType Type;
	public readonly ulong ModifierFlags;
	
	public PropertyDeclaration(
		string name,
		CompilationType type,
		MethodDeclaration? getter,
		MethodDeclaration? setter,
		CompilationType parentType,
		ulong flags)
	{
		Name = name;
		For = parentType;
		Getter = getter;
		Setter = setter;
		ModifierFlags = flags;
		Type = type;
	}

	public readonly StringBuilder BuildMeta()
	{
		StringBuilder meta = new(100);

		meta.Append(".prop ");
		meta.Append(IMemberDeclaration.GenerateModifierString(ModifierFlags));

		if (Getter is not null) meta.Append('g');
		if (Setter is not null) meta.Append('s');

		meta.Append(Type.ToFullyQualifiedString());
		meta.Append(' ');
		meta.Append(Name);
		
		return meta;
	}
}