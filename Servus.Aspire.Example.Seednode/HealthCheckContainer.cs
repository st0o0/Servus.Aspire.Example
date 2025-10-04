using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Servus.Core.Application.HealthChecks;

namespace Servus.Aspire.Example.Seednode;

public class HealthCheckContainer : HealthCheckSetupContainer
{
    protected override void SetupHealthChecks(IHealthChecksBuilder builder)
    {
        builder.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
    }
}