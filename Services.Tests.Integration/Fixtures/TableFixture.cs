using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace SchoolChallenge.Fixtures
{
    /// <summary>
    /// This test fixture will setup and teardown a real table
    /// </summary>
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
