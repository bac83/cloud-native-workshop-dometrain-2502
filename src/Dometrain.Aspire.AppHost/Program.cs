
var builder = DistributedApplication.CreateBuilder(args);

var mainDbUsername = builder.AddParameter("postgres-username");
var mainDbPassword = builder.AddParameter("postgres-password");

var mainDb = builder.AddPostgres("main-db", mainDbUsername, mainDbPassword, port: 5432)
    .WithDataVolume()
    .AddDatabase("dometrain");

var cartAccount = builder.AddAzureCosmosDB("cosmosdb")
    .AddCosmosDatabase("cartdb");

var cartDb = cartAccount.AddContainer("carts", "/pk");

builder.AddProject<Projects.Dometrain_Monolith_Api>("dometrain-api")
    .WithReference(mainDb)
    .WithReference(cartAccount)
    .WaitFor(cartAccount)
    .WaitFor(mainDb);

var app = builder.Build();
    
app.Run();
