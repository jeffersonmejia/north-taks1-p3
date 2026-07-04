using NorthwindStore.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddProjectSecrets();
builder.Services.AddNorthwindStore(builder.Configuration);

var app = builder.Build();

app.UseNorthwindStorePipeline();

await app.SeedIdentityAsync();
app.Run();
