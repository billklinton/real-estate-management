using MediatR;
using OperationResult;
using RealEstateManagement.Domain.Mappers;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Exceptions;
using RealEstateManagement.Shareable.Requests;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Domain.Handlers
{
    public class GetRealEstateRequestHandler : IRequestHandler<GetRealEstateRequest, Result<BaseResponse<List<RealEstateDto>>>>
    {
        private readonly IRealEstateRepository _realEstateRepository;

        public GetRealEstateRequestHandler(IRealEstateRepository realEstateRepository)
        {
            this._realEstateRepository = realEstateRepository;
        }

        public async Task<Result<BaseResponse<List<RealEstateDto>>>> Handle(GetRealEstateRequest request, CancellationToken cancellationToken)
        {
            var realEstates = await _realEstateRepository.GetAsync(request.Page, request.PageSize, request.State, request.City, request.SaleMode);
            if (!realEstates.Any())
                return new NotFoundException("Real Estates were not found");

            return Result.Success(new BaseResponse<List<RealEstateDto>>(200, "Success", RealEstateMapper.ToListDto(realEstates)));
        }      
    }
}
