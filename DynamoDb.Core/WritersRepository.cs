using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DynamoDb.Contracts;
using DynamoDb.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoDb.Core
{
    public class WritersRepository : IWritersRepository
    {
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;

        public WritersRepository()
        {
            _client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(_client);
        }

        public async Task Add(Writer entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            entity.AddedOn = DateTime.Now;

            await _context.SaveAsync(entity);
        }

        public async Task<WriterOutputModel> All(string paginationToken = "")
        {
            // Get the Table ref from the Model
            var table = _context.GetTargetTable<Writer>();

            // If there's a PaginationToken
            // Use it in the Scan options
            // to fetch the next set
            var scanOps = new ScanOperationConfig();
            
            if (!string.IsNullOrEmpty(paginationToken))
            {
                scanOps.PaginationToken = paginationToken;
            }

            // returns the set of Document objects
            // for the supplied ScanOptions
            var results = table.Scan(scanOps);
            List<Document> data = await results.GetNextSetAsync();

            // transform the generic Document objects
            // into our Entity Model
            IEnumerable<Writer> Writers = _context.FromDocuments<Writer>(data);

            // Pass the PaginationToken
            // if available from the Results
            // along with the Result set
            return new WriterOutputModel
            {
                PaginationToken = results.PaginationToken,
                Writers = Writers,
                ResultsType = ResultsType.List
            };

            /* The Non-Pagination approach */
            //var scanConditions = new List<ScanCondition>() { new ScanCondition("Id", ScanOperator.IsNotNull) };
            //var searchResults = _context.ScanAsync<Writer>(scanConditions, null);
            //return await searchResults.GetNextSetAsync();
        }

        public async Task<IEnumerable<Writer>> Find(InputModel searchReq)
        {
            var scanConditions = new List<ScanCondition>();
            if (!string.IsNullOrEmpty(searchReq.Username))
                scanConditions.Add(new ScanCondition("Username", ScanOperator.Equal, searchReq.Username));
            if (!string.IsNullOrEmpty(searchReq.EmailAddress))
                scanConditions.Add(new ScanCondition("EmailAddress", ScanOperator.Equal, searchReq.EmailAddress));
            if (!string.IsNullOrEmpty(searchReq.Name))
                scanConditions.Add(new ScanCondition("Name", ScanOperator.Equal, searchReq.Name));

            return await _context.ScanAsync<Writer>(scanConditions, null).GetRemainingAsync();
        }

        public async Task Remove(Guid writerId)
        {
            await _context.DeleteAsync<Writer>(writerId);
        }

        public async Task<Writer> Single(Guid writerId)
        {
            return await _context.LoadAsync<Writer>(writerId); //.QueryAsync<Writer>(writerId.ToString()).GetRemainingAsync();
        }

        public async Task Update(Guid writerId, Writer entity)
        {
            var writer = await Single(writerId);

            writer.EmailAddress = entity.EmailAddress;
            writer.Username = entity.Username;
            writer.Name = entity.Name;

            await _context.SaveAsync(writer);
        }
    }
}
