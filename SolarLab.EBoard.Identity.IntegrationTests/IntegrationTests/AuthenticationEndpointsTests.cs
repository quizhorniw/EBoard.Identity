using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Login;
using SolarLab.EBoard.Identity.Domain.Entities;
using SolarLab.EBoard.Identity.Infrastructure.Persistence;

namespace SolarLab.EBoard.Identity.IntegrationTests.IntegrationTests;

public class AuthenticationEndpointsTests : IClassFixture<IdentityWebApplicationFactory>
{
    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnAuthToken()
    {
        // Arrange
        var factory = new IdentityWebApplicationFactory();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<AppDbContext>();

            var user = new User(
                "test@mail.com",
                "+79180576819",
                "Иван",
                "Иванов",
                "C2C074D158BAB27FBC2A36FE54C3D8D6A0423D2D49EF9C8C46E9CC146CA27A66" +
                "-C7F246A15FC4E102DA694332BB1B6793");

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        var url = "/api/auth/login";
        var request = new
        {
            Email = "test@mail.com",
            Password = "1234abcdef"
        };

        // Act
        var response = await client.PostAsJsonAsync(url, request);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.Contains('.', result.AccessToken);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_SetRefreshToken()
    {
        // Arrange
        var factory = new IdentityWebApplicationFactory();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<AppDbContext>();

            var user = new User(
                "test@mail.com",
                "+79180576819",
                "Иван",
                "Иванов",
                "C2C074D158BAB27FBC2A36FE54C3D8D6A0423D2D49EF9C8C46E9CC146CA27A66" +
                "-C7F246A15FC4E102DA694332BB1B6793");

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        var url = "/api/auth/login";
        var request = new
        {
            Email = "test@mail.com",
            Password = "1234abcdef"
        };

        // Act
        var response = await client.PostAsJsonAsync(url, request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookies));
        Assert.Contains("refreshToken", cookies.ElementAtOrDefault(0));
    }

    [Fact]
    public async Task Login_NonExistentUser_ReturnNullAccessTokenAndBadRequest()
    {
        // Arrange
        var client = new IdentityWebApplicationFactory().CreateClient();

        var url = "/api/auth/login";
        var request = new
        {
            Email = "test@mail.com",
            Password = "1234abcdef"
        };
        
        // Act
        var response = await client.PostAsJsonAsync(url, request);
        var result = (await response.Content.ReadFromJsonAsync<LoginResponse>())!;
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(result.AccessToken);
    }

    [Fact]
    public async Task Login_IncorrectPassword_ReturnNullAccessTokenAndBadRequest()
    {
        // Arrange
        var factory = new IdentityWebApplicationFactory();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<AppDbContext>();

            var user = new User(
                "test@mail.com",
                "+79180576819",
                "Иван",
                "Иванов",
                "C2C074D158BAB27FBC2A36FE54C3D8D6A0423D2D49EF9C8C46E9CC146CA27A66" +
                "-C7F246A15FC4E102DA694332BB1B6793");

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
        
        var url = "/api/auth/login";
        var request = new
        {
            Email = "test@mail.com",
            Password = "totally-wrong-password"
        };
        
        // Act
        var response = await client.PostAsJsonAsync(url, request);
        var result = (await response.Content.ReadFromJsonAsync<LoginResponse>())!;
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(result.AccessToken);
    }
}