
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Dometrain_Monolith_Api>("dometrain-api");

var app = builder.Build();
    
app.Run();
