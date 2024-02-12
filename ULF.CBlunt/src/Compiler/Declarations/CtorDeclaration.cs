using System.Text;

namespace ULF.CBlunt.Compiler.Declarations;

public readonly struct CtorDeclaration : IMemberDeclaration
{
	public readonly string NativeCode;
	public readonly byte OverloadNumber;
	public readonly CompilationType For;
	public readonly CompilationType[] Args;
	public readonly ulong ModifierFlags;
	
	public CtorDeclaration(
		string nativeCode,
		CompilationType parentType,
		CompilationType[] args,
		ulong flags,
		byte overloadNumber = 0)
	{
		NativeCode = nativeCode;
		For = parentType;
		Args = args;
		ModifierFlags = flags;
		OverloadNumber = overloadNumber;
	}

	public readonly StringBuilder BuildMeta()
	{
		StringBuilder meta = new(100);

		meta.Append(".ctor ");
		meta.Append(IMemberDeclaration.GenerateModifierString(ModifierFlags));
		meta.Append('(');

		foreach (var argType in Args)
		{
			meta.Append(argType.ToFullyQualifiedString());
			meta.Append(',');
		}

		meta.Remove(meta.Length-1, 1);

		meta.Append(')');

		return meta;
	}
}