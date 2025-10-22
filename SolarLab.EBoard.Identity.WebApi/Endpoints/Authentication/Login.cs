using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Login;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

internal sealed class Login : IEndpoint
{
    internal sealed record LoginRequest(string Email, string Password);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
            return Results.Ok(result);
        })
            .AllowAnonymous();
    }
}