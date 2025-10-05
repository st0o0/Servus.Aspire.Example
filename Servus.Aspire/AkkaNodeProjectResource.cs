using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Servus.Aspire;

public interface IAkkaNodeResource : IResource;

public class AkkaNodeProjectResource : ProjectResource, IAkkaNodeResource
{
    public AkkaNodeProjectResource(string name) : base(name)
    {
        this.
    }
}

public static class Extensions
{
    public static IResourceBuilder<AkkaNodeProjectResource> AddClusterProject<T>(
        this IDistributedApplicationBuilder builder, [ResourceName] string name)
    {
    }
}