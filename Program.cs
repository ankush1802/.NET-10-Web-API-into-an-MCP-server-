using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<ProductTools>();

var app = builder.Build();

// Demo only: create the database and seed data on startup
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.MapControllers();
app.MapMcp("/mcp");

app.Run();
