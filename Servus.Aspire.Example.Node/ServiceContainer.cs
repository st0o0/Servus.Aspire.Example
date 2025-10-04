using Akka.Cluster.Hosting;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Servus.Core.Application.Startup;

namespace Servus.Aspire.Example.Node;

public class ServiceContainer : IServiceSetupContainer, ILoggingSetupContainer
{
    public void SetupServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDiscovery();
        services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource()
                    .AddAspNetCoreInstrumentation(
                        //options =>
                        // Exclude health check requests from tracing
                        // options.Filter = context =>
                        //     !context.Request.Path.StartsWithSegments("/healthz")
                        //     && !context.Request.Path.StartsWithSegments("/alive")
                    )
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        var useOtlpExporter = !string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            services.AddOpenTelemetry().UseOtlpExporter();
        }

        var options = configuration.GetRequiredSection(HuginClusterOptions.SectionName).Get<HuginClusterOptions>()!;
        services.AddAkka("TEST", builder =>
        {
            builder.WithActorSystemLivenessCheck()
                .WithAkkaClusterReadinessCheck();
            // Setup Akka.Remote
            builder.WithRemoting("0.0.0.0", options.Port, options.Hostname);

            // Setup Akka.Cluster
            builder.WithClustering(new ClusterOptions
            {
                SeedNodes = options.SeedNodes.ToArray(),
                Roles = options.Roles.ToArray(),
            });
        });
    }

    public void SetupLogging(ILoggingBuilder builder)
    {
        builder.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });
    }
}