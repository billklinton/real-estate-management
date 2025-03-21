using MediatR;
using OperationResult;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Shareable.Requests
{
    public record GetByIdRequest(Guid? Id) : IRequest<Result<BaseResponse<RealEstateDto>>>;
}
