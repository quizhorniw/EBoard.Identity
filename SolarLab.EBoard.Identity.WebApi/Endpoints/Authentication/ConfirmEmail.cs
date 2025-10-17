using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.ConfirmEmail;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

internal sealed class ConfirmEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/confirmEmail", async (
            Guid userId, 
            [FromQuery(Name = "token")] string confirmationToken,
            IMediator mediator,
            CancellationToken cancellationToken) => 
            {
                var command = new ConfirmEmailCommand(userId, confirmationToken);
                await mediator.Send(command, cancellationToken);
                return Results.Ok();
            })
            .AllowAnonymous();
    }
}