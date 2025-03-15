namespace RealEstateManagement.Domain.Repositories
{
    public interface IBaseRepository<T>
    {
        Task InserManyAsync(IEnumerable<T> entity);
    }
}
