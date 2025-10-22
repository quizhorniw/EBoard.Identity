using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Login;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

internal sealed class Login : IEndpoint
{
    internal sealed record LoginRequest(string Email, string Password);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login",
            [SwaggerOperation("Login into the system")]
            [SwaggerResponse(200, "Success", typeof(LoginResponse))]
            [SwaggerResponse(400, "Unauthorized access")]
            [SwaggerResponse(500, "Internal server error")]
            async (
                LoginRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
            return Results.Ok(result);
        })
            .AllowAnonymous();
    }
}