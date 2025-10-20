using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Time;
using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.Infrastructure.Authentication;

public class TokenProvider : ITokenProvider
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public TokenProvider(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public string CreateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            Environment.GetEnvironmentVariable("JWT_SECRET")!));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            ]),
            Expires = _dateTimeProvider.UtcNow.AddMinutes(int.Parse(
                Environment.GetEnvironmentVariable("JWT_ACCESS_EXPIRATION_MINUTES")!)),
            SigningCredentials = credentials,
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
        };

        var handler = new JsonWebTokenHandler();

        var token = handler.CreateToken(tokenDescriptor);
        return token;
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        return new RefreshToken(
            userId,
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(128)),
            _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow.AddDays(int.Parse(
                Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRATION_DAYS")!)));
    }

    public string GenerateEmailConfirmationToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace("+", "");
    }
}