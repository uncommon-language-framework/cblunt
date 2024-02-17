using System.Text;

namespace ULF.CBlunt.Compiler.Declarations;

public readonly struct MethodDeclaration : IMemberDeclaration
{
	public readonly string Name;
	public readonly string NativeCode;
	public readonly byte OverloadNumber;
	public readonly CompilationType For;
	public readonly CompilationType[] Args;
	public readonly string[] ArgNames;
	public readonly CompilationType ReturnType;
	public readonly ulong ModifierFlags;
	
	public MethodDeclaration(
		string name,
		string nativeCode,
		CompilationType parentType,
		CompilationType[] args,
		string[] argNames,
		CompilationType returnType,
		ulong flags,
		byte overloadNumber = 0)
	{
		Name = name;
		NativeCode = nativeCode;
		For = parentType;
		Args = args;
		ArgNames = argNames;
		ReturnType = returnType;
		ModifierFlags = flags;
		OverloadNumber = overloadNumber;
	}

	public readonly StringBuilder BuildMeta()
	{
		StringBuilder meta = new(100);

		meta.Append(IMemberDeclaration.GenerateModifierString(ModifierFlags));
		meta.Append(ReturnType.ToFullyQualifiedString());
		meta.Append(' ');
		meta.Append(Name);
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