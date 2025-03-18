using MediatR;
using Microsoft.AspNetCore.Mvc;
using OperationResult;
using RealEstateManagement.Shareable.Exceptions;
using RealEstateManagement.Shareable.Requests;

namespace RealEstateManagement.Api
{
    public static class Endpoints
    {
        public static void MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/add-from-csvfile", static async (IMediator mediator, [FromForm] IFormFileCollection file) =>
            {
                var stream = file[0].OpenReadStream();
                return await mediator.SendCommand(new AddFromCsvFileRequest(stream));
            })
            .DisableAntiforgery()
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK);

            app.MapPost("api/v1/login", static async (IMediator mediator, [FromBody] LoginRequest request) =>
                await mediator.SendCommand(request));
        }

        static async Task<IResult> SendCommand<T>(this IMediator mediator, IRequest<Result<T>> request, Func<T, IResult> result = null)
            => await mediator.Send(request) switch
            {
                (true, var response, _) => result is not null ? result(response!) : Results.Ok(response),
                var (_, _, error) => HandleError(error!)
            };

        static IResult HandleError(Exception error)
            => error switch
            {
                UnauthorizedException => Results.Unauthorized(),
                Shareable.Exceptions.ApplicationException => Results.InternalServerError(),
                _ => Results.InternalServerError(500)
            };
    }
}
