using System.Collections.Generic;
using System.Reflection;
using PostSharp.Extensibility;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Sdk.Extensibility;
using PostSharp.Sdk.Extensibility.Tasks;

namespace PostSharp.Community.DeepSerializable.Weaver
{
    public class DeepSerializableTask : Task
    {
        private readonly HashSet<TypeDefDeclaration> examinedTypes = new HashSet<TypeDefDeclaration>();

        public override bool Execute()
        {
            IAnnotationRepositoryService annotationService = this.Project.GetService<IAnnotationRepositoryService>();
            IEnumerator<IAnnotationInstance> annotations =
                annotationService.GetAnnotationsOfType(typeof(DeepSerializableAttribute), false, true);
            while (annotations.MoveNext())
            {
                IAnnotationInstance annotation = annotations.Current;
                MakeSerializableRecursively(annotation.TargetElement as TypeDefDeclaration);
            }

            return base.Execute();
        }

        private void MakeSerializableRecursively(TypeDefDeclaration type)
        {
            if (examinedTypes.Contains(type))
            {
                // We've already analyzed this type.
                return;
            }

            examinedTypes.Add(type);
            
            EnsureSerializable(type);
            
            foreach (FieldDefDeclaration field in type.Fields)
            {
                IdentifyAndMakeSerializableRecursively(field.FieldType, field);
            }

            IdentifyAndMakeSerializableRecursively(type.BaseType, type);
        }

        private void IdentifyAndMakeSerializableRecursively(ITypeSignature type, MessageLocation location)
        {
            switch (type.TypeSignatureElementKind)
            {
                case TypeSignatureElementKind.Intrinsic:
                    // This works automatically.
                    break;
                case TypeSignatureElementKind.TypeDef:
                    // Let's hope that all the types in this assembly are typedefs. From what I remember, it's possible
                    // that some of them are presented as typerefs which would cause them to be just verified, which
                    // isn't ideal, but we'll see.
                    MakeSerializableRecursively((TypeDefDeclaration) type);
                    break;
                case TypeSignatureElementKind.TypeRef:
                    // We are unable to affect another assembly.
                    VerifySerializable((TypeRefDeclaration) type, location);
                    break;
                case TypeSignatureElementKind.GenericInstance:
                    GenericTypeInstanceTypeSignature
                        genericInstanceSignature = type as GenericTypeInstanceTypeSignature;
                    IdentifyAndMakeSerializableRecursively(genericInstanceSignature.ElementType, location);
                    foreach (ITypeSignature argument in genericInstanceSignature.GenericArguments)
                    {
                        IdentifyAndMakeSerializableRecursively(argument, location);
                    }
                    break;
                case TypeSignatureElementKind.Array:
                    ArrayTypeSignature arraySignature = type as ArrayTypeSignature;
                    IdentifyAndMakeSerializableRecursively(arraySignature.ElementType, location);
                    break;
                default:
                    Message.Write(location, SeverityType.Warning, "DSER002",
                        "A type signature (" + type + ") has an unrecognized kind (" + type.TypeSignatureElementKind +
                        ").");
                    break;
            }
        }

        private void VerifySerializable(TypeRefDeclaration type, MessageLocation location)
        {
            if (!IsSerializable(type))
            {
                // This does not work properly on .NET Core.
                // PostSharp has difficulty understanding that some classes in .NET Core are serializable.
                
                // Message.Write(location, SeverityType.Warning, "DSER001",
                //     "A type (" + type.Name +
                //     ") is not serializable, but it's not in the same assembly so I cannot modify it.");
            }
        }

        private void EnsureSerializable(TypeDefDeclaration type)
        {
            if (!IsSerializable(type))
            {
                type.Attributes |= TypeAttributes.Serializable;
            }
        }

        private static bool IsSerializable(IType type)
        {
            return (type.Attributes & TypeAttributes.Serializable) != 0;
        }
    }
}