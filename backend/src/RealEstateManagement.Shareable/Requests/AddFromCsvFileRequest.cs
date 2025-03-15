using MediatR;
using OperationResult;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Shareable.Requests
{
    public record AddFromCsvFileRequest(Stream Stream) : IRequest<Result<BaseResponse>>
    {
    }
}
