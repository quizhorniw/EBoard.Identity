using MediatR;

namespace SolarLab.EBoard.Identity.Application.Users.GetById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;