using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Logout;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Authentication;

public class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout",
            [SwaggerOperation("Logout from the system")]
            [SwaggerResponse(200, "Success")]
            [SwaggerResponse(401, "Unauthorized access")]
            [SwaggerResponse(500, "Internal server error")]
            async (
                HttpRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (!request.Cookies.TryGetValue("refreshToken", out _))
                {
                    return Results.Unauthorized();
                }

                await mediator.Send(new LogoutCommand(), cancellationToken);
                return Results.Ok();
            })
            .RequireAuthorization();
    }
}