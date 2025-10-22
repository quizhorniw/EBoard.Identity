using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Refresh;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

internal sealed class Refresh : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh",
            [SwaggerOperation("Refresh access token")]
            [SwaggerResponse(200, "Success", typeof(RefreshTokenResponse))]
            [SwaggerResponse(400, "Unauthorized access")]
            [SwaggerResponse(500, "Internal server error")]
            async (
                HttpRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) => 
            {
                if (!request.Cookies.TryGetValue("refreshToken", out var oldToken))
                {
                    return Results.Unauthorized();
                }

                var result = await mediator.Send(new RefreshTokenCommand(oldToken), cancellationToken);
                return Results.Ok(result);
            })
            .AllowAnonymous();
    }
}