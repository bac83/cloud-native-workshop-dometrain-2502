
var builder = DistributedApplication.CreateBuilder(args);

var mainDbUsername = builder.AddParameter("postgres-username");
var mainDbPassword = builder.AddParameter("postgres-password");

var mainDb = builder.AddPostgres("main-db", mainDbUsername, mainDbPassword, port: 5432)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    //.WithPgAdmin()
    .AddDatabase("dometrain");

var cartAccount = builder.AddAzureCosmosDB("cosmosdb")
    .AddCosmosDatabase("cartdb");

cartAccount.AddContainer("carts", "/pk");


builder.AddProject<Projects.Dometrain_Monolith_Api>("dometrain-api")
    .WithReference(mainDb)
    .WithReference(cartAccount)
    .WaitFor(cartAccount)
    .WaitFor(mainDb)
    .WithReplicas(5);

var app = builder.Build();
    
app.Run();
