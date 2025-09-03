using MediatR;
using SolarLab.EBoard.Identity.Application.Users.Login;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Users;

internal sealed class Login : IEndpoint
{
    internal sealed record Request(string Email, string Password);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new LoginUserCommand(request.Email, request.Password), cancellationToken);
            return Results.Ok(result);
        })
            .AllowAnonymous();
    }
}