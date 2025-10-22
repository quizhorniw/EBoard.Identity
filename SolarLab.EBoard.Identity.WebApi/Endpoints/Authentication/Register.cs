using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Register;
using SolarLab.EBoard.Identity.WebApi.Endpoints.Users;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

internal sealed class Register : IEndpoint
{
    internal sealed record RegisterRequest(
        string Email,
        string? PhoneNumber,
        string FirstName, 
        string LastName,
        string Password
    );
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register",
            [SwaggerOperation("Register new user")]
            [SwaggerResponse(204, "User registered successfully")]
            [SwaggerResponse(500, "Internal server error")]
            async (
                RegisterRequest request,
                IMediator mediator,
                CancellationToken token) => 
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