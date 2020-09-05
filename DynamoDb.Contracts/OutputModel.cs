namespace DynamoDb.Contracts
{
    public class OutputModel
    {
        public ResultsType ResultsType { get; set; }
        public string PaginationToken { get; set; }
    }
}
