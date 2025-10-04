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
                    .AddAspNetCoreInstrumentation(options =>
                        // Exclude health check requests from tracing
                        options.Filter = context =>
                            !context.Request.Path.StartsWithSegments("/healthz")
                            && !context.Request.Path.StartsWithSegments("/alive")
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

        var akkaOptions = new AkkaOptions();
        configuration.GetSection("Akka").Bind(akkaOptions);
        services.AddAkka("TEST", builder =>
        {
            // Setup Akka.Remote
            builder.WithRemoting(opt =>
            {
                opt.PublicHostName = akkaOptions.Hostname;
                opt.PublicPort = akkaOptions.Port;
                opt.HostName = "0.0.0.0";
                opt.Port = akkaOptions.Port;
            });

            // Setup Akka.Cluster
            builder.WithClustering(new ClusterOptions
            {
                SeedNodes = configuration
                    .GetSection("CLUSTER_SEEDS")
                    .GetChildren()
                    .Select(c => c.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Cast<string>()
                    .ToArray()
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