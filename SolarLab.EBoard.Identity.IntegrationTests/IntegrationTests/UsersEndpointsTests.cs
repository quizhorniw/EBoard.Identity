using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolarLab.EBoard.Identity.Application.CQRS.Users;
using SolarLab.EBoard.Identity.Domain.Entities;
using SolarLab.EBoard.Identity.Infrastructure.Persistence;
using SolarLab.EBoard.Identity.IntegrationTests.Helpers;

namespace SolarLab.EBoard.Identity.IntegrationTests.IntegrationTests;

public class UsersEndpointsTests : IClassFixture<IdentityWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IdentityWebApplicationFactory _factory;

    public UsersEndpointsTests(IdentityWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetUserById_ExistingId_ReturnSuccessAndCorrectUser()
    {
        // Arrange
        Guid userId;
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<AppDbContext>();

            var user = new User(
                "test@mail.com",
                "+79180576819",
                "Иван",
                "Иванов",
                "hash");
            userId = user.Id;

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
        
        var url = "/api/users/" + userId;

        // Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("test@mail.com", result.Email);
        Assert.Equal("+79180576819", result.PhoneNumber);
        Assert.Equal("Иван", result.FirstName);
        Assert.Equal("Иванов", result.LastName);
    }

    [Fact]
    public async Task GetUserById_NonExistingId_ReturnsNullAndNotFound()
    {
        // Arrange
        var userId = Guid.Parse("9c66c442-1de2-4beb-92f0-bb4eff442b66");
        var url = "/api/users/" + userId;

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await Assert.ThrowsAsync<JsonException>(() => response.Content.ReadFromJsonAsync<UserDto>());
    }

    [Fact]
    public async Task GetUserById_WithoutAuthentication_ReturnsNullAndUnauthorized()
    {
        // Arrange
        var noAuthClient = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                WebApplicationFactoryHelper.RemoveDbContext<AppDbContext>(services);
            
                services.AddSingleton<DbConnection>(_ =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<AppDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });

                services.AddHostedService<DatabaseInitializerHostedService>();
            });

            builder.UseEnvironment("IntegrationTests");
        }).CreateClient();
        
        var userId = Guid.Parse("9c66c442-1de2-4beb-92f0-bb4eff442b66");
        var url = "/api/users/" + userId;

        // Act
        var response = await noAuthClient.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await Assert.ThrowsAsync<JsonException>(() => response.Content.ReadFromJsonAsync<UserDto>());
    }
}