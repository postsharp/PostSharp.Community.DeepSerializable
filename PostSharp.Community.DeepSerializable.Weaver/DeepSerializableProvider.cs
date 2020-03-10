using PostSharp.Community.DeepSerializable.Weaver;
using PostSharp.Sdk.Extensibility;
using PostSharp.Sdk.Extensibility.Configuration;

[assembly: AssemblyProjectProvider(typeof(DeepSerializableProvider))]

namespace PostSharp.Community.DeepSerializable.Weaver
{
    public class DeepSerializableProvider : IProjectConfigurationProvider
    {
        public ProjectConfiguration GetProjectConfiguration() => new ProjectConfiguration()
        {
            TaskTypes = new TaskTypeConfigurationCollection
            {
                new TaskTypeConfiguration("DeepSerializableTask", project => new DeepSerializableTask())
                {
                    Phase = "Transform",
                    Dependencies = new DependencyConfigurationCollection
                    {
                        new DependencyConfiguration("AnnotationRepository", true),
                        new DependencyConfiguration("AspectWeaver", false)
                        {
                            Position = DependencyPosition.Before
                        }
                    }
                }
            }
        };
    }
}