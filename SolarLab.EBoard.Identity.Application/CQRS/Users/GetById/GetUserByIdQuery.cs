using MediatR;

namespace SolarLab.EBoard.Identity.Application.CQRS.Users.GetById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;