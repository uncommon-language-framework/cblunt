using System.Text;
using ULF.CBlunt.Compiler.DefaultTypes;

namespace ULF.CBlunt.Compiler.Declarations;

public class GenericTypeStub : CompilationType
{
	public new readonly string ClassName;
	public new readonly CompilationType ImmediateBase;
	public new CompilationType[] ImplementedInterfaces;
	public readonly bool HasConstructorConstraint;
	public readonly bool HasStructConstraint;
	public readonly bool HasClassConstraint;

	public GenericTypeStub(string name, CompilationType immediateBase, CompilationType[] interfaces, bool constructorConstraint = false, bool structConstraint = false, bool classConstraint = false)
	{
		ClassName = name;
		HasConstructorConstraint = constructorConstraint;
		HasStructConstraint = structConstraint;
		HasClassConstraint = classConstraint;
		ImmediateBase = immediateBase;
		ImplementedInterfaces = interfaces;
	}

	public GenericTypeStub(string name, CompilationType immediateBase, bool constructorConstraint = false, bool structConstraint = false, bool classConstraint = false)
		: this(name, immediateBase, Array.Empty<CompilationType>(), constructorConstraint, structConstraint, classConstraint) { }
	
	public GenericTypeStub(string name, CompilationType[] interfaces, bool constructorConstraint = false, bool structConstraint = false, bool classConstraint = false)
		: this(name, DefaultMetadata.System.Object, interfaces, constructorConstraint, structConstraint, classConstraint) { }
	
	public override StringBuilder ToFullyQualifiedString(bool forTypeDecl = false, bool withGenericArgs = false)
	{
		return new StringBuilder(ClassName);
	}
}
