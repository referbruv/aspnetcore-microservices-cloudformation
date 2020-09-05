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
    public class ReadersRepository : IReadersRepository
    {
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;

        public ReadersRepository()
        {
            _client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(_client);
        }

        public async Task Add(Reader entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            entity.AddedOn = DateTime.Now;

            await _context.SaveAsync(entity);
        }

        public async Task<ReaderOutputModel> All(string paginationToken = "")
        {
            // Get the Table ref from the Model
            var table = _context.GetTargetTable<Reader>();

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
            IEnumerable<Reader> readers = _context.FromDocuments<Reader>(data);

            // Pass the PaginationToken
            // if available from the Results
            // along with the Result set
            return new ReaderOutputModel
            {
                PaginationToken = results.PaginationToken,
                Readers = readers,
                ResultsType = ResultsType.List
            };

            /* The Non-Pagination approach */
            //var scanConditions = new List<ScanCondition>() { new ScanCondition("Id", ScanOperator.IsNotNull) };
            //var searchResults = _context.ScanAsync<Reader>(scanConditions, null);
            //return await searchResults.GetNextSetAsync();
        }

        public async Task<IEnumerable<Reader>> Find(InputModel searchReq)
        {
            var scanConditions = new List<ScanCondition>();
            if (!string.IsNullOrEmpty(searchReq.Username))
                scanConditions.Add(new ScanCondition("Username", ScanOperator.Equal, searchReq.Username));
            if (!string.IsNullOrEmpty(searchReq.EmailAddress))
                scanConditions.Add(new ScanCondition("EmailAddress", ScanOperator.Equal, searchReq.EmailAddress));
            if (!string.IsNullOrEmpty(searchReq.Name))
                scanConditions.Add(new ScanCondition("Name", ScanOperator.Equal, searchReq.Name));

            return await _context.ScanAsync<Reader>(scanConditions, null).GetRemainingAsync();
        }

        public async Task Remove(Guid readerId)
        {
            await _context.DeleteAsync<Reader>(readerId);
        }

        public async Task<Reader> Single(Guid readerId)
        {
            return await _context.LoadAsync<Reader>(readerId); //.QueryAsync<Reader>(readerId.ToString()).GetRemainingAsync();
        }

        public async Task Update(Guid readerId, Reader entity)
        {
            var reader = await Single(readerId);

            reader.EmailAddress = entity.EmailAddress;
            reader.Username = entity.Username;
            reader.Name = entity.Name;

            await _context.SaveAsync(reader);
        }
    }
}
