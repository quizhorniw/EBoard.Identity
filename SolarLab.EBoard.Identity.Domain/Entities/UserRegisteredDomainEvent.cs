using MediatR;

namespace SolarLab.EBoard.Identity.Domain.Entities;

public sealed record UserRegisteredDomainEvent(Guid UserId) : INotification;