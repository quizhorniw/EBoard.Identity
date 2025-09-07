using MediatR;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Register;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterHandler(IUsersRepository usersRepository, IPasswordHasher passwordHasher)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User(
            request.Email,
            request.PhoneNumber,
            request.FirstName,
            request.LastName,
            _passwordHasher.Hash(request.Password)
            );
        
        await _usersRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}