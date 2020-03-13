using System.Collections.Generic;
using System.Reflection;
using PostSharp.Extensibility;
using PostSharp.Sdk.CodeModel;
using PostSharp.Sdk.CodeModel.TypeSignatures;
using PostSharp.Sdk.Extensibility;
using PostSharp.Sdk.Extensibility.Configuration;
using PostSharp.Sdk.Extensibility.Tasks;
#pragma warning disable 649

namespace PostSharp.Community.DeepSerializable.Weaver
{
    [ExportTask(Phase = TaskPhase.Transform, TaskName = nameof(DeepSerializableTask))]
    [TaskDependency(TaskNames.AspectWeaver, IsRequired = false, Position = DependencyPosition.After)]
    public class DeepSerializableTask : Task
    {
        [ImportService] 
        private IAnnotationRepositoryService annotationService;
    
        private readonly HashSet<TypeDefDeclaration> examinedTypes = new HashSet<TypeDefDeclaration>();

        public override bool Execute()
        {
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
            
            MakeSerializable(type);
            
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
                    // This works automatically for most, but:
                    // TODO: have an error for object, IntPtr.
                    break;

                case TypeSignatureElementKind.TypeDef:
                    TypeDefDeclaration typeDef = (TypeDefDeclaration) type;
                    if (typeDef.DeclaringAssembly == this.Project.Module.DeclaringAssembly)
                    {
                        MakeSerializableRecursively( typeDef );
                    }
                    else
                    {
                        VerifySerializable( typeDef, location );
                    }
                    break;

                case TypeSignatureElementKind.TypeRef:
                    IdentifyAndMakeSerializableRecursively( type.GetTypeDefinition(), location );
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
                    // Other possible signature types can be ignored:
                    //  Pointers: they cannot be serialized
                    //  Custom modifiers: they should not be used on fields.
                    break;
            }
        }

        private void VerifySerializable( TypeDefDeclaration type, MessageLocation location)
        {
            if (!IsSerializable(type))
            {
                 Message.Write(location, SeverityType.Warning, "DSER001",
                     "The type {0} is not serializable, but it's not in the same assembly so I cannot modify it.", type);
            }
        }

        private void MakeSerializable(TypeDefDeclaration type)
        {
            if (!IsSerializable(type))
            {
                type.Attributes |= TypeAttributes.Serializable;
            }
        }

        private static bool IsSerializable( TypeDefDeclaration type ) => (type.Attributes & TypeAttributes.Serializable) != 0;
    }
}