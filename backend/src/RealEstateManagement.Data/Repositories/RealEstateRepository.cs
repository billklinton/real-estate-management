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

        public async Task InserManyAsync(IEnumerable<RealEstate> entity)
        {
            await _collection.InsertManyAsync(entity);
        }
    }
}
