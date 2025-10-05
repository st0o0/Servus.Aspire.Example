var builder = DistributedApplication.CreateBuilder(args);

var mongoDb = builder.AddMongoDB("mongo");

var seedNode = builder.AddProject<Projects.Servus_Aspire_Example_Seednode>("seednode")
    .WithEnvironment("ClusterOptions__SeedNodes__1", "akka.tcp://TEST@localhost:14001")
    .WithEnvironment("ClusterOptions__Roles__1", "seednode")
    .WithEnvironment("ClusterOptions__Port", "14001")
    .WithEnvironment("ClusterOptions__Hostname", "localhost")
    .WithHttpEndpoint()
    .WithHttpHealthCheck("/healthz")
    .WithReference(mongoDb)
    .WaitFor(mongoDb);

var node = builder.AddProject<Projects.Servus_Aspire_Example_Node>("node")
    .WithEnvironment("ClusterOptions__SeedNodes__1", "akka.tcp://TEST@localhost:14001")
    .WithEnvironment("ClusterOptions__Roles__1", "node")
    .WithEnvironment("ClusterOptions__Port", "14002")
    .WithEnvironment("ClusterOptions__Hostname", "localhost")
    
    .WithHttpEndpoint()
    .WithHttpHealthCheck("/healthz")
    .WithReference(seedNode)
    .WithReference(mongoDb)
    .WaitFor(seedNode)
    .WaitFor(mongoDb);

builder.Build().Run();