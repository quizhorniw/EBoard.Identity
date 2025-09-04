using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.Domain.Interfaces;

public interface IRefreshTokensRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}