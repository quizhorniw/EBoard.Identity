using MediatR;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Domain.Entities;
using SolarLab.EBoard.Identity.Domain.Interfaces;

namespace SolarLab.EBoard.Identity.Application.Users.Register;

internal sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUsersRepository usersRepository, IPasswordHasher passwordHasher)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
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