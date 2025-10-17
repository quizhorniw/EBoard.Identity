using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.Application.Abstractions.Persistence;

public interface IUsersRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}