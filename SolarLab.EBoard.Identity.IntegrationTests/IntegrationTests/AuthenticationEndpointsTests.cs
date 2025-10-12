using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Login;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Refresh;
using SolarLab.EBoard.Identity.Domain.Entities;
using SolarLab.EBoard.Identity.Infrastructure.Persistence;
using Xunit.Abstractions;

namespace SolarLab.EBoard.Identity.IntegrationTests.IntegrationTests;

public class AuthenticationEndpointsTests : IClassFixture<IdentityWebApplicationFactory>
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AuthenticationEndpointsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

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
    public async Task Refresh_WithValidRefreshToken_ReturnAccessTokenAndSuccess()
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

        var oldToken = await GetRefreshTokenFromApi(client, "test@mail.com", "1234abcdef");
        client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={oldToken}");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/refresh", new { OldToken = oldToken });
        var result = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.Contains('.', result.AccessToken);
    }

    private static async Task<string?> GetRefreshTokenFromApi(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            return null;
        }

        foreach (var cookie in cookies)
        {
            var parts = cookie.Split(';').Select(p => p.Trim()).ToArray();
            var kv = parts[0].Split(['='], 2);
            if (kv.Length == 2 && string.Equals(kv[0], "refreshToken", StringComparison.OrdinalIgnoreCase))
            {
                return Uri.UnescapeDataString(kv[1]);
            }
        }

        return null;
    }

    [Fact]
    public async Task Refresh_WithoutRefreshTokenCookie_ReturnNullAndUnauthorized()
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

        var oldToken = await GetRefreshTokenFromApi(client, "test@mail.com", "1234abcdef");
        // No refresh token cookie

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/refresh", new { OldToken = oldToken });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await Assert.ThrowsAsync<JsonException>(() =>
            response.Content.ReadFromJsonAsync<RefreshTokenResponse>());
    }

    [Fact]
    public async Task Register_Valid_ReturnsUserIdAndSuccess()
    {
        // Arrange
        var client = new IdentityWebApplicationFactory().CreateClient();
        
        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = "test@mail.com",
            PhoneNumber = "+79180576819",
            FirstName = "Иван",
            LastName = "Иванов",
            Password = "1234abcdef"
        });
        var result = await response.Content.ReadFromJsonAsync<Guid>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
    }
}