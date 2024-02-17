using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using ULF.CBlunt.Compiler.Declarations;
using ULF.CBlunt.Compiler.DefaultTypes;

namespace ULF.CBlunt.Compiler;


public class CompilationUnit
{
	public static readonly CompilationUnit Empty = new(string.Empty, string.Empty);
	public readonly List<CompilationType> Types = new();
	public readonly string AssemblyName;
	public readonly string Source;
	public string Metadata = string.Empty;
	
	public CompilationUnit(string assemblyName, string sourceCode)
	{
		AssemblyName = assemblyName;
		Source = sourceCode;
	}

	public StringBuilder ToNativeCode()
	{
		StringBuilder builder = new(100000);

		builder.AppendLine("#include <StdULR.hpp>");
		builder.Append('\n', 2);

		List<string> declNames = new();

		foreach (var type in Types)
		{
			for (int i = 0; i < type.CtorDecls.Count; i++)
			{
				var ctorDecl = type.CtorDecls[i];

				StringBuilder declName = new(20);

				builder.Append("void ");

				declName.Append("overload");
				declName.Append(i);
				declName.Append('_');
				declName.Append(type.ToNativeRepr());
				declName.Append('_');
				declName.Append("ctor");

				string declNameString = declName.ToString();

				builder.Append(declNameString);

				declNames.Add(declNameString);

				builder.Append('(');

				if (type.IsStruct)
				{
					builder.Append("sizeof_");
					builder.Append(type.ToNativeRepr());
					builder.Append("* self");
				}
				else builder.Append("void* self");
			
				for (int j = 0; j < ctorDecl.Args.Length; j++)
				{
					var argType = ctorDecl.Args[j];

					builder.Append(',');
					
					if (argType.IsStruct)
					{
						builder.Append("sizeof_");
						builder.Append(argType.ToNativeRepr());
						builder.Append(' ');
						builder.Append(ctorDecl.ArgNames[j]);
						continue;
					}

					builder.Append("void* ");
					builder.Append(ctorDecl.ArgNames[j]);
				}

				builder.Append(")\n{");

				builder.Append(ctorDecl.NativeCode);
				builder.Append('}');

				builder.Append('\n', 2);
			}

			int overloadCounter = 0;
			string methodName = string.Empty;

			for (int i = 0; i < type.MethodDecls.Count; i++)
			{
				var methodDecl = type.MethodDecls[i];

				if (methodDecl.Name == methodName) overloadCounter++;
				else
				{
					methodName = methodDecl.Name;
					overloadCounter = 0;
				}

				if (methodDecl.ReturnType.IsStruct)
				{
					if (methodDecl.ReturnType == DefaultMetadata.System.Void) builder.Append("void");
					else
					{
						builder.Append("sizeof_");
						builder.Append(methodDecl.ReturnType.ToNativeRepr());
					}
				}
				else builder.Append("void*");

				builder.Append(' ');

				StringBuilder declName = new(20);

				declName.Append("overload");
				declName.Append(overloadCounter);
				declName.Append('_');
				declName.Append(type.ToNativeRepr());
				declName.Append('_');
				declName.Append(methodDecl.Name);

				string declNameString = declName.ToString();

				builder.Append(declNameString);
				
				declNames.Add(declNameString);
				
				builder.Append('(');

				if ((methodDecl.ModifierFlags & ModifierFlags.Static) == 0) // if not static...
				{
					if (type.IsStruct)
					{
						builder.Append("sizeof_");
						builder.Append(methodDecl.For.ToNativeRepr());
						builder.Append("* self");
					}
					else builder.Append("void* self");
				}
				
				for (int j = 0; j < methodDecl.Args.Length; j++)
				{
					var argType = methodDecl.Args[j];

					// if not static OR not first arg, add comma delim
					if ((methodDecl.ModifierFlags & ModifierFlags.Static) == 0 || j != 0) builder.Append(", ");
					
					if (argType.IsStruct)
					{
						builder.Append("sizeof_");
						builder.Append(argType.ToNativeRepr());
						builder.Append(' ');
						builder.Append(methodDecl.ArgNames[j]);
						continue;
					}

					builder.Append("void* ");
					builder.Append(methodDecl.ArgNames[j]);
				}

				builder.Append(")\n{");

				builder.Append(methodDecl.NativeCode);
				builder.Append("\n\n}");

				builder.Append('\n', 2);
			}

			if (type.DtorDecl.HasValue)
			{
				builder.Append("void ");

				StringBuilder declName = new(20);

				declName.Append(type.ToNativeRepr());
				declName.Append('_');
				declName.Append("dtor");

				string declNameString = declName.ToString();

				builder.Append(declNameString);

				declNames.Add(declNameString);
				
				builder.Append('(');
				
				if (type.IsStruct)
				{
					builder.Append("sizeof_");
					builder.Append(type.ToNativeRepr());
					builder.Append("* self");
				}
				else builder.Append("void* self");

				builder.Append(")\n{");

				builder.Append(type.DtorDecl.Value.NativeCode);
				builder.Append('}');

				builder.Append('\n', 2);
			}
		}
		/*
			Insert native code
		*/

		builder.Append("char ulrmeta[] = ");
		builder.Append(
			SymbolDisplay.FormatLiteral(Metadata, true)
		);
		builder.Append(';');
		builder.Append('\n', 2);
		builder.Append("void* ulraddr[] = {\n");
		
		for (int i = 0; i < declNames.Count; i++)
		{
			builder.Append(declNames[i]);

			if (i != declNames.Count-1)
			{
				builder.Append(',');
				builder.AppendLine();
			}
		}

		builder.Append('}');
		builder.Append(';');
		builder.AppendLine();

		return builder;
	}
}