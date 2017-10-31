using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace SchoolChallenge.Fixtures
{
    public class TableFixture : IDisposable
    {
        public CloudTable Table { get; private set;}

        public TableFixture(string connectionString, string tableName)
        {
            Table = CloudStorageAccount.Parse(connectionString).CreateCloudTableClient().GetTableReference(tableName);

            var createTableResult = Table.CreateIfNotExistsAsync().Result;
        }

        public void Dispose()
        {
            Table.DeleteAsync();
        }
    }
}
