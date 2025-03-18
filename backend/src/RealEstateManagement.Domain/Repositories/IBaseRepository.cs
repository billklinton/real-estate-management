namespace RealEstateManagement.Domain.Repositories
{
    public interface IBaseRepository<T>
    {
        Task InserManyAsync(List<T> entity);
    }
}
