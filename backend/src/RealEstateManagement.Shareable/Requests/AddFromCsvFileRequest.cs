using MediatR;
using Microsoft.AspNetCore.Http;
using OperationResult;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Shareable.Requests
{
    public record AddFromCsvFileRequest(IFormFileCollection Files) : IRequest<Result<BaseResponse<string>>>;
}
