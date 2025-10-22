using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.ConfirmEmail;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

internal sealed class ConfirmEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/confirmEmail",
                [SwaggerOperation("Confirm email")]
                [SwaggerResponse(200, "Success")]
                [SwaggerResponse(404, "Requested user was not found")]
                [SwaggerResponse(400, "Invalid email confirmation token")]
                [SwaggerResponse(500, "Internal server error")]
                async (
                    [SwaggerParameter("User ID")]
                    Guid userId, 
                    [FromQuery(Name = "token")]
                    [SwaggerParameter("Email confirmation token")]
                    string confirmationToken,
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