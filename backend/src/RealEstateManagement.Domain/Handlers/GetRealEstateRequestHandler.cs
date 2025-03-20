using MediatR;
using OperationResult;
using RealEstateManagement.Domain.Mappers;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Exceptions;
using RealEstateManagement.Shareable.Requests;
using RealEstateManagement.Shareable.Responses;
using System.Collections.Generic;

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
            if(request.Page < 0)
                return HandleError(new InvalidArgumentException("Page must be greater than or equal to 0"));

            if (request.PageSize < 1 || request.PageSize > 100)
                return HandleError(new InvalidArgumentException("PageSize must be between 1 and 100"));

            var realEstates = await _realEstateRepository.GetAsync(request.Page, request.PageSize, request.State, request.City, request.SaleMode);
            if (!realEstates.Any())
                return HandleError(new NotFoundException("Real Estates were not found"));

            return Result.Success(new BaseResponse<List<RealEstateDto>>(200, "Success", RealEstateMapper.ToListDto(realEstates)));
        }

        private static Result<BaseResponse<List<RealEstateDto>>> HandleError(Exception e)
            => Result.Error<BaseResponse<List<RealEstateDto>>>(e);        
    }
}
