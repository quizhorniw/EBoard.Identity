using MediatR;
using Microsoft.Extensions.Logging;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.ConfirmEmail;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand>
{
    private readonly ILogger<ConfirmEmailHandler> _logger;
    private readonly IUsersRepository _usersRepository;

    public ConfirmEmailHandler(ILogger<ConfirmEmailHandler> logger, IUsersRepository usersRepository)
    {
        _logger = logger;
        _usersRepository = usersRepository;
    }

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogError("No user found with id = {Id}", request.UserId);
            throw new KeyNotFoundException("Invalid UserID");
        }

        if (user.IsConfirmed)
        {
            _logger.LogError("User with id = {Id} is already confirmed", user.Id);
            return;
        }

        if (request.ConfirmationToken != user.ConfirmationToken)
        {
            _logger.LogError("Provided confirmation token = {Token} does not belong to user with id = {Id}", 
                request.ConfirmationToken, user.Id);
            throw new ArgumentException("Invalid confirmation token");
        }
        
        user.ConfirmEmail();
        await _usersRepository.UpdateAsync(user, cancellationToken);
    }
}