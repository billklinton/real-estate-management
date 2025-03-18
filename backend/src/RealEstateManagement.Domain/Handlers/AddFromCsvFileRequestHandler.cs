using MediatR;
using OperationResult;
using RealEstateManagement.Domain.Services;
using RealEstateManagement.Kafka;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Requests;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Domain.Handlers
{
    public class AddFromCsvFileRequestHandler : IRequestHandler<AddFromCsvFileRequest, Result<BaseResponse>>
    {
        private readonly IProducer _producer;
        private readonly ICsvService _csvService;

        public AddFromCsvFileRequestHandler(IProducer producer, ICsvService csvService)
        {
            _producer = producer;
            _csvService = csvService;
        }

        public async Task<Result<BaseResponse>> Handle(AddFromCsvFileRequest request, CancellationToken cancellationToken)
        {
            var realEstates = _csvService.ReadCSV(request.Stream);
            var batchSize = 1000;
            var batchedList = new List<RealEstateDto>();

            foreach (var item in realEstates)
            {
                batchedList.Add(item);

                if (batchedList.Count >= batchSize)
                {
                    await _producer.SendMessageAsync(batchedList);
                    batchedList.Clear();
                }
            }

            if (batchedList.Count > 0)            
                await _producer.SendMessageAsync(batchedList);            

            return Result.Success(new BaseResponse(200, "OK"));
        }
    }
}
