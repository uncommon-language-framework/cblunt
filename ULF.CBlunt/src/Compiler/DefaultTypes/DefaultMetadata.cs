using ULF.CBlunt.Compiler.Declarations;

namespace ULF.CBlunt.Compiler.DefaultTypes;

public static class DefaultMetadata
{
	public const ulong DEFAULT_REF_OBJ_SIZE = 8;
	public static readonly CompilationUnit StandardLibraryAssembly = new("System.Runtime.dll", "");
	public static readonly CompilationType[] DefaultIncludedTypes;

	// };

	public class System
	{
		public static readonly CompilationType Object = new("System", "Object", CompilationType.Type.Class, StandardLibraryAssembly, CompilationType.NoBase, ModifierFlags.Public, DEFAULT_REF_OBJ_SIZE);
		public static readonly CompilationType Array = new("System", "Array", CompilationType.Type.Class, StandardLibraryAssembly, Object, ModifierFlags.Abstract | ModifierFlags.Public, DEFAULT_REF_OBJ_SIZE);
		public static readonly CompilationType ValueType = new("System", "ValueType", CompilationType.Type.Class, StandardLibraryAssembly, Object, ModifierFlags.Abstract | ModifierFlags.Public, DEFAULT_REF_OBJ_SIZE);
		public static readonly CompilationType Void = new("System", "Void", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Public, 0);

		public static readonly CompilationType SByte = new("System", "SByte", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 1);
		public static readonly CompilationType Byte = new("System", "Byte", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 1);
		public static readonly CompilationType Int16 = new("System", "Int16", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 2);
		public static readonly CompilationType UInt16 = new("System", "UInt16", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 2);
		public static readonly CompilationType Int32 = new("System", "Int32", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 4);
		public static readonly CompilationType UInt32 = new("System", "UInt32", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 4);
		public static readonly CompilationType Int64 = new("System", "Int64", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 8);
		public static readonly CompilationType UInt64 = new("System", "UInt64", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 8);
		public static readonly CompilationType Char = new("System", "Char", CompilationType.Type.Struct, StandardLibraryAssembly, ValueType, ModifierFlags.Readonly | ModifierFlags.Public, 2);
		// "String" will be considered as an array of Char with different method bindings
		public static readonly CompilationType String = new("System", "String", CompilationType.Type.ArrayType, StandardLibraryAssembly, Array, ModifierFlags.Sealed | ModifierFlags.Public, DEFAULT_REF_OBJ_SIZE); // this has a special size since elems are allocated like an array of char[]
}


	static DefaultMetadata()
	{
		DefaultIncludedTypes = new[] {
			System.Object, System.Array, System.ValueType, System.Void, System.String,
			System.SByte, System.Byte, System.Int16, System.UInt16,
			System.Int32, System.UInt32, System.Int64, System.UInt64
		};
	}
}