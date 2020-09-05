using System;
using System.Threading.Tasks;

namespace DynamoDb.Contracts.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> Single(Guid id);
        Task Add(T entity);
        Task Remove(Guid id);
        Task Update(Guid id, T entity);
    }
}
