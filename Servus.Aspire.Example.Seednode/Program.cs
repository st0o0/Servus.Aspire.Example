using Microsoft.Extensions.Configuration;
using Servus.Aspire.Example.Seednode;
using Servus.Core.Application.Startup;

await AppBuilder.Create()
    .WithSetup<ServiceContainer>()
    .WithSetup<HealthCheckContainer>()
    .Build().RunAsync();

public class AkkaOptions
{
    [ConfigurationKeyName("hostname")] public string Hostname { get; set; } = "localhost";

    [ConfigurationKeyName("port")] public int Port { get; set; } = 14884;
}