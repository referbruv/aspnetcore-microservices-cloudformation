using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDb.Contracts
{
    public class ReaderOutputModel : OutputModel
    {
        public IEnumerable<Reader> Readers { get; set; }
    }
}
