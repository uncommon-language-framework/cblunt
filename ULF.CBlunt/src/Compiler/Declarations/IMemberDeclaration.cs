using System.Text;

namespace ULF.CBlunt.Compiler.Declarations;

public interface IMemberDeclaration
{
	StringBuilder BuildMeta();

	static StringBuilder GenerateModifierString(ulong modifiers)
	{
		StringBuilder modString = new();

		if ((modifiers & ModifierFlags.Public) != 0)
			modString.Append('p');
		if ((modifiers & ModifierFlags.Protected) != 0)
			modString.Append('t');
		if ((modifiers & ModifierFlags.Internal) != 0)
			modString.Append('i');
		if ((modifiers & ModifierFlags.Static) != 0)
			modString.Append('s');
		if ((modifiers & ModifierFlags.Readonly) != 0)
			modString.Append('r');
		if ((modifiers & ModifierFlags.Virtual) != 0)
			modString.Append('v');
		if ((modifiers & ModifierFlags.Abstract) != 0)
			modString.Append('a');
		if ((modifiers & ModifierFlags.Partial) != 0)
			modString.Append('l');
		if ((modifiers & ModifierFlags.Extern) != 0)
			modString.Append('e');
		if ((modifiers & ModifierFlags.Sealed) != 0)
			modString.Append('d');

		return modString;
	}
}