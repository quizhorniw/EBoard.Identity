using MediatR;
using SolarLab.EBoard.Identity.Application.Users.Register;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Users;

public class Register : IEndpoint
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
        app.MapPost("/users/register", async (Request request, IMediator mediator, CancellationToken token) =>
        {
            var command = new RegisterUserCommand(
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