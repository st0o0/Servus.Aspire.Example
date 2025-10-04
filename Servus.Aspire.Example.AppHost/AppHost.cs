var builder = DistributedApplication.CreateBuilder(args);

var seedNode = builder.AddProject<Projects.Servus_Aspire_Example_Seednode>("seednode")
    .WithEndpoint();

var node = builder.AddProject<Projects.Servus_Aspire_Example_Node>("node")
    .WithReference(,);


builder.Build().Run();