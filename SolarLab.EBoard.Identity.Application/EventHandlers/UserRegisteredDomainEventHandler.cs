using MediatR;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Messaging;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.Application.EventHandlers;

public class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IMessageProducer _messageProducer;

    public UserRegisteredDomainEventHandler(IUsersRepository usersRepository, ITokenProvider tokenProvider,
        IMessageProducer messageProducer)
    {
        _usersRepository = usersRepository;
        _tokenProvider = tokenProvider;
        _messageProducer = messageProducer;
    }

    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(notification.UserId, cancellationToken);
        if (user == null)
        {
            return;
        }

        var token = _tokenProvider.GenerateEmailConfirmationToken();
        
        user.SetConfirmationToken(token);
        await _usersRepository.UpdateAsync(user, cancellationToken);
        
        var confirmationUrl = $"http://localhost:5279/api/auth/confirmEmail?userId={user.Id}&token={token}";
        
        var message = new
        {
            to = new[] { user.Email },
            subject = "Email confirmation",
            content = $"""
                       <p>Thank you for registering! Please confirm your email by clicking the link below:</p>
                       <p><a href="{confirmationUrl}">Confirm Email</a></p>
                       """,
            isHtml = true
        };

        await _messageProducer.SendAsync(message, cancellationToken);
    }
}