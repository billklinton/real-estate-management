using MongoDB.Driver;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Models;

namespace RealEstateManagement.Data.Repositories
{
    public class RealEstateRepository : IRealEstateRepository
    {
        private readonly IMongoCollection<RealEstate> _collection;

        public RealEstateRepository(IMongoCollection<RealEstate> collection)
        {
            _collection = collection;
        }

        public async Task<List<RealEstate>> GetAsync(int page, int pageSize, string? state = null, string? city = null, string? saleMode = null)
        {
            var filter = Builders<RealEstate>.Filter.Empty;

            if (!string.IsNullOrEmpty(saleMode))
                filter &= Builders<RealEstate>.Filter.Eq(x => x.SaleMode, saleMode);

            if (!string.IsNullOrEmpty(city))
                filter &= Builders<RealEstate>.Filter.Eq(x => x.City, city);

            if (!string.IsNullOrEmpty(state))
                filter &= Builders<RealEstate>.Filter.Eq(x => x.State, state);

            return await _collection.Find(filter)
                                          .Skip(page * pageSize)
                                          .Limit(pageSize)
                                          .ToListAsync();
        }

        public async Task<RealEstate> GetByIdAsync(Guid? id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();        

        public async Task InserManyAsync(List<RealEstate> entity)
        {
            await _collection.InsertManyAsync(entity);
        }
    }
}
