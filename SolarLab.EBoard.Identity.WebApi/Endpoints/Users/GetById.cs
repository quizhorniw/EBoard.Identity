using MediatR;
using SolarLab.EBoard.Identity.Application.CQRS.Users.GetById;

namespace SolarLab.EBoard.Identity.WebApi.Endpoints.Users;

internal sealed class GetById : IEndpoint
{
    internal const string EndpointName = "GetUserById"; 
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
            .WithName(EndpointName)
            .RequireAuthorization();
    }
}