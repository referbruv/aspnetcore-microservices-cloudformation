using Amazon.DynamoDBv2.DataModel;
using System;

namespace DynamoDb.Contracts.Interfaces
{
    [DynamoDBTable("test_writers")]
    public class Writer
    {
        public Writer()
        {

        }

        public Writer(string emailAddress, string username, string name)
        {
            EmailAddress = emailAddress;
            Username = username;
            Name = name;
        }

        [DynamoDBProperty("id")]
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty("name")]
        public string Name { get; set; }

        [DynamoDBProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [DynamoDBProperty("userName")]
        public string Username { get; set; }

        [DynamoDBProperty("addedOn")]
        public DateTime AddedOn { get; set; }
    }
}