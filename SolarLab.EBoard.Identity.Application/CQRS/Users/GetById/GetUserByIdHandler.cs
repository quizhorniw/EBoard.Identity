using MediatR;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;

namespace SolarLab.EBoard.Identity.Application.CQRS.Users.GetById;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserByIdHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _usersRepository.GetByIdAsync(request.Id, cancellationToken);
        return result is null
            ? null
            : new UserDto(result.Id, result.Email, result.PhoneNumber, result.FirstName, result.LastName);
    }
}