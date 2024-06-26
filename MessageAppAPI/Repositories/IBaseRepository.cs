using MessageAppAPI.Entities;

namespace MessageAppAPI.Repositories;

public interface IBaseRepository<T>:IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll(bool tracking = true);
    Task<T> GetByIdAsync(string id,bool tracking = true);
    Task<bool> AddAsync(T model);
    bool Remove(T model);
    Task<bool> RemoveAsync(string id);
    bool Update(T model);
    Task<int> SaveAsync();
    Task AddRangeAsync(IEnumerable<T> model);
}