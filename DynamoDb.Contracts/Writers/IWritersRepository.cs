using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoDb.Contracts.Interfaces
{
    public interface IWritersRepository : IRepository<Writer>
    {
        Task<WriterOutputModel> All(string paginationToken = "");
        Task<IEnumerable<Writer>> Find(InputModel searchRequest);
    }
}
