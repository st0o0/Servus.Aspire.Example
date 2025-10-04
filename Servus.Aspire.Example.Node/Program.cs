using Microsoft.Extensions.Configuration;
using Servus.Aspire.Example.Node;
using Servus.Core.Application.Startup;

await AppBuilder.Create()
    .WithSetup<ServiceContainer>()
    .WithSetup<HealthCheckContainer>()
    .Build().RunAsync();

public sealed class HuginClusterOptions
{
    internal const string EnvironmentVariableNamePort = "ACTORSYSTEM_PORT";
    internal const string EnvironmentVariableNameHostname = "ACTORSYSTEM_HOSTNAME";

    internal const string SectionName = "ClusterOptions";

    public string Hostname { get; set; } = string.Empty;
    public int Port { get; set; }

    public List<string> Roles { get; set; } = new();
    public List<string> SeedNodes { get; set; } = new();
}