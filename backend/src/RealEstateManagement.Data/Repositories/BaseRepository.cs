using MongoDB.Driver;
using RealEstateManagement.Domain.Repositories;

namespace RealEstateManagement.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public BaseRepository(IMongoCollection<T> collection) => _collection = collection;

        public async Task InserManyAsync(List<T> entity)
        {
            await _collection.InsertManyAsync(entity);
        }
    }
}
