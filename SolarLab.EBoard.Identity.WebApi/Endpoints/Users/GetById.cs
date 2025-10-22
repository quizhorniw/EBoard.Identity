using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Users;
using SolarLab.EBoard.Identity.Application.CQRS.Users.GetById;
using Swashbuckle.AspNetCore.Annotations;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Users;

internal sealed class GetById : IEndpoint
{
    internal const string EndpointName = "GetUserById"; 
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id:guid}",
                [SwaggerOperation("Get user by ID")]
                [SwaggerResponse(200, "Success", typeof(UserDto))]
                [SwaggerResponse(404, "Requested user was not found")]
                [SwaggerResponse(500, "Internal server error")]
                async (
                    [SwaggerParameter("User ID")]
                    Guid id,
                    IMediator mediator,
                    CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
            .WithName(EndpointName)
            .AllowAnonymous();
    }
}