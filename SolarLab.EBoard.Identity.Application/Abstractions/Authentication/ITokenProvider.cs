using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string CreateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
}