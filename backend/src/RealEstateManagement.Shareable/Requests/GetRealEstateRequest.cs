using MediatR;
using OperationResult;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Shareable.Requests
{
    public record GetRealEstateRequest(int Page, int PageSize, string? State = null, string? City = null, string? SaleMode = null) : IRequest<Result<BaseResponse<List<RealEstateDto>>>>;
}
