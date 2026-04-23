using AccessManager.Application;
using AccessManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
{
    Name = "AccessManager.Api",
    Status = "Running"
}));

app.Run();
