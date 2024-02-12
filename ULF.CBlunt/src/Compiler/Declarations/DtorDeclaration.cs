using System.Text;

namespace ULF.CBlunt.Compiler.Declarations;

public readonly struct DtorDeclaration : IMemberDeclaration
{
	public readonly string NativeCode;
	public readonly byte OverloadNumber;
	public readonly CompilationType For;
	public readonly ulong ModifierFlags;
	
	public DtorDeclaration(
		string nativeCode,
		CompilationType parentType,
		ulong flags)
	{
		NativeCode = nativeCode;
		For = parentType;
		ModifierFlags = flags;
	}

	public readonly StringBuilder BuildMeta()
	{
		return new(".dtor");
	}
}