using System.Reflection;
using SolarLab.EBoard.Identity.Application;
using SolarLab.EBoard.Identity.Infrastructure;
using SolarLab.EBoard.Identity.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

var apiGroup = app.MapGroup("/api");
app.MapEndpoints(apiGroup);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MigrateDb();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

public partial class Program { }