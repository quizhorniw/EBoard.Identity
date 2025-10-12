using MediatR;
using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Register;

public sealed class UserRegisteredHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Email user to confirm registration
    }
}