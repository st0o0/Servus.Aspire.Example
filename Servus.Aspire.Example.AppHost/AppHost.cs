using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

var mongoDb = builder.AddMongoDB("mongo");

var seedNode = builder.AddProject<Projects.Servus_Aspire_Example_Seednode>("seednode")
    .WithEndpoint(name: "remote", env: "Akka__Port")
    .WithEndpoint("akka", endpoint =>
    {
        endpoint.Protocol = ProtocolType.Tcp;
        endpoint.TargetHost = $"TEST@{endpoint.TargetHost}";
        endpoint.UriScheme = "akka.tcp";
    })
    .WithHttpEndpoint()
    .WithHttpHealthCheck("/healthz")
    .WithReference(mongoDb)
    .WaitFor(mongoDb);

var node = builder.AddProject<Projects.Servus_Aspire_Example_Node>("node")
    .WithEndpoint(name: "remote", env: "Akka__Port")
    .WithHttpEndpoint()
    .WithHttpHealthCheck("/healthz")
    .WithReference(seedNode)
    .WithReference(mongoDb)
    .WaitFor(mongoDb);

builder.Build().Run();