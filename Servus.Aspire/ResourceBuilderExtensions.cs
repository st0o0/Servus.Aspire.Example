using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Servus.Aspire;

public static class ResourceBuilderExtensions
{
    public static IResourceBuilder<ProjectResource> AddServusProject<TProject>(
        this IDistributedApplicationBuilder builder, [ResourceName] string name)
        where TProject : IProjectMetadata, new()
    { 
        
        return builder.AddProject<TProject>(name, _ => { });
    }

    public static IResourceBuilder<ProjectResource> AddServusProject(this IDistributedApplicationBuilder builder,
        [ResourceName] string name, string projectPath)
    {
        
        return builder.AddProject(name, projectPath, _ => { });
    }
}