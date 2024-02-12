using System.Text;

namespace ULF.CBlunt.Compiler.Declarations;

public readonly struct FieldDeclaration : IMemberDeclaration
{
	public readonly string Name;
	public readonly uint Offset;
	public readonly CompilationType For;
	public readonly CompilationType Type;
	public readonly ulong ModifierFlags;
	
	public FieldDeclaration(
		string name,
		uint offset,
		CompilationType parentType,
		CompilationType type,
		ulong flags)
	{
		Name = name;
		Offset = offset;
		For = parentType;
		Type = type;
		ModifierFlags = flags;
	}

	public readonly StringBuilder BuildMeta()
	{
		StringBuilder meta = new(100);

		meta.Append(".fldv ");
		meta.Append(IMemberDeclaration.GenerateModifierString(ModifierFlags));
		meta.Append(Type.ToFullyQualifiedString());
		meta.Append(' ');
		meta.Append(Name);

		return meta;
	}
}