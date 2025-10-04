using Akka.Cluster.Hosting;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // bind environment configuration passed in from Aspire
        var akkaOptions = new AkkaOptions();
        context.Configuration.GetSection("Akka").Bind(akkaOptions);
        
        services.AddAkka("SimpleCluster", builder =>
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
                
            });
        });
    })
    .UseConsoleLifetime()
    .RunConsoleAsync();
    
public class AkkaOptions
{
    [ConfigurationKeyName("hostname")]
    public string Hostname { get; set; } = "localhost";
    
    [ConfigurationKeyName("port")]
    public int Port { get; set; } = 14884;
}