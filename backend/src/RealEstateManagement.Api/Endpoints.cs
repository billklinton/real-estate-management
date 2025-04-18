﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using OperationResult;
using RealEstateManagement.Shareable.Exceptions;
using RealEstateManagement.Shareable.Requests;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Api
{
    public static class Endpoints
    {
        public static void MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/add-from-csvfile", static async (IMediator mediator, [FromForm] IFormFileCollection file) =>
                await mediator.SendCommand(new AddFromCsvFileRequest(file)))
            .DisableAntiforgery()
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK);

            app.MapPost("api/v1/login", static async (IMediator mediator, [FromBody] LoginRequest request) =>
                await mediator.SendCommand(request));

            app.MapGet("api/v1/getById", static async (IMediator mediator, [FromQuery] Guid? id) =>
                await mediator.SendCommand(new GetByIdRequest(id)))
            .RequireAuthorization();

            app.MapGet("api/v1/get", static async (IMediator mediator, [FromQuery] string? state = null, [FromQuery] string? city = null, [FromQuery] string? saleMode = null, [FromQuery] int page = 0, [FromQuery] int pageSize = 20) =>
                await mediator.SendCommand(new GetRealEstateRequest(page, pageSize, state, city, saleMode)))
            .RequireAuthorization();
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
                DataInvalidException e => Results.BadRequest(new BaseResponse<IEnumerable<string>>(400, e.Message, e.Errors)),
                NotFoundException e => Results.NotFound(new BaseResponse<string>(404, e.Message)),
                UnauthorizedException e => Results.Json(new BaseResponse<string>(401, e.Message), statusCode: 401),
                Shareable.Exceptions.ApplicationException e => Results.InternalServerError(new BaseResponse<string>(500, e.Message)),
                _ => Results.InternalServerError(new BaseResponse<string>(500, error.Message))
            };
    }
}
