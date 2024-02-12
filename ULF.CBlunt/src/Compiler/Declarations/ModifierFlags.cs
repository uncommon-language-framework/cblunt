namespace ULF.CBlunt.Compiler.Declarations;

public static class ModifierFlags // yes, an enum could have been used, but this avoids tedious (ulong) casts when checking for bitflags using &
{
	public const ulong
		Private = 0,
		Public = 1 << 0,
		Protected = 1 << 1,
		Internal = 1 << 2,
		Static = 1 << 3,
		Readonly = 1 << 4,
		Virtual = 1 << 5,
		Abstract = 1 << 6,
		Partial = 1 << 7,
		Extern = 1 << 8,
		Sealed = 1 << 9;
}