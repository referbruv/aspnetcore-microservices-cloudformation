using DynamoDb.Contracts.Interfaces;
using System.Collections.Generic;

namespace DynamoDb.Contracts
{
    public class WriterOutputModel : OutputModel
    {
        public IEnumerable<Writer> Writers { get; set; }
    }
}