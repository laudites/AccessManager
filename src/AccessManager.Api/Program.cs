using AccessManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
