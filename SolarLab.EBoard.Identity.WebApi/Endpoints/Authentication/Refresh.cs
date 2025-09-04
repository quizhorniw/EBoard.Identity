using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Refresh;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

internal sealed class Refresh : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh", async (HttpRequest request, IMediator mediator, CancellationToken cancellationToken) =>
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