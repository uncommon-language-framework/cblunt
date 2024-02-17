using System.Text;

namespace ULF.CBlunt.Compiler.Declarations;

public readonly struct CtorDeclaration : IMemberDeclaration
{
	public readonly string NativeCode;
	public readonly byte OverloadNumber;
	public readonly CompilationType For;
	public readonly CompilationType[] Args;
	public readonly string[] ArgNames;
	public readonly ulong ModifierFlags;
	
	public CtorDeclaration(
		string nativeCode,
		CompilationType parentType,
		CompilationType[] args,
		string[] argNames,
		ulong flags,
		byte overloadNumber = 0)
	{
		NativeCode = nativeCode;
		For = parentType;
		Args = args;
		ArgNames = argNames;
		ModifierFlags = flags;
		OverloadNumber = overloadNumber;
	}

	public readonly StringBuilder BuildMeta()
	{
		StringBuilder meta = new(100);

		meta.Append(".ctor ");
		meta.Append(IMemberDeclaration.GenerateModifierString(ModifierFlags));
		meta.Append('(');

		for (int i = 0; i < Args.Length; i++)
		{
			meta.Append(Args[i].ToFullyQualifiedString());
			if (i != Args.Length-1) meta.Append(',');
		}
		
		meta.Append(')');

		return meta;
	}
}