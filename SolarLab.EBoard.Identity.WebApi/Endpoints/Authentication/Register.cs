using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Register;
using SolarLab.EBoard.Identity.WebApi.Endpoints.Users;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

internal sealed class Register : IEndpoint
{
    internal sealed record Request(
        string Email,
        string? PhoneNumber,
        string FirstName, 
        string LastName,
        string Password
    );
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (Request request, IMediator mediator, CancellationToken token) =>
        {
            var command = new RegisterCommand(
                request.Email,
                request.PhoneNumber,
                request.FirstName,
                request.LastName,
                request.Password
                );
            
            var result = await mediator.Send(command, token);
            return Results.CreatedAtRoute(GetById.EndpointName, new { id = result }, result);
        })
            .AllowAnonymous();
    }
}