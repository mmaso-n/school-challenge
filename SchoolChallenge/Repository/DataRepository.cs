using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SchoolChallenge.Repository.Data;
using System.Threading.Tasks;

namespace SchoolChallenge.Repository
{
    public interface IDataRepository
    {
        Task<TableQuerySegment<Student>> GetStudentsAsync(string school, TableContinuationToken continuationToken = default(TableContinuationToken));
        Task<TableQuerySegment<Teacher>> GetTeachersAsync(string school, TableContinuationToken continuationToken = default(TableContinuationToken));
        Task UpsertAsync(string tableName, TableEntity toAdd);
        Task DeleteAsync(string tableName, TableEntity toDelete);
    }

    public class DataRepository : IDataRepository
    {
        private readonly Config _settings;
        private CloudStorageAccount _cloudStorageAccount;

        public DataRepository(IOptions<Config> settings)
        {
            _settings = settings.Value;
        }

        public DataRepository(Config settings)
        {
            _settings = settings;
        }

        private CloudStorageAccount GetStorageAccount()
        {
            if (_cloudStorageAccount == null)
                _cloudStorageAccount = CloudStorageAccount.Parse(_settings.StorageConnectionString);

            return _cloudStorageAccount;
        }

        public async Task<TableQuerySegment<Student>> GetStudentsAsync(string school, TableContinuationToken continuationToken = default(TableContinuationToken))
        {
            var table = GetStorageAccount().
                CreateCloudTableClient().
                GetTableReference(_settings.StudentTable);

            var query = new TableQuery<Student>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, school));

            return await table.ExecuteQuerySegmentedAsync(query, continuationToken);
        }

        public async Task<TableQuerySegment<Teacher>> GetTeachersAsync(string school, TableContinuationToken continuationToken = default(TableContinuationToken))
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_settings.TeacherTable);

            var query = new TableQuery<Teacher>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, school));

            return await table.ExecuteQuerySegmentedAsync(query, continuationToken);
        }

        public async Task UpsertAsync(string tableName, TableEntity toAdd)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(tableName);

            var operation = TableOperation.InsertOrReplace(toAdd);
            
            await table.ExecuteAsync(operation);
        }

        public async Task DeleteAsync(string tableName, TableEntity toDelete)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(tableName);

            toDelete.ETag = "*";

            var operation = TableOperation.Delete(toDelete);

            await table.ExecuteAsync(operation);
        }
    }
}
