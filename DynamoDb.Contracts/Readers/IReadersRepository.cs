using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoDb.Contracts.Interfaces
{
    public interface IReadersRepository : IRepository<Reader>
    {
        Task<ReaderOutputModel> All(string paginationToken = "");
        Task<IEnumerable<Reader>> Find(InputModel searchRequest);
    }
}
