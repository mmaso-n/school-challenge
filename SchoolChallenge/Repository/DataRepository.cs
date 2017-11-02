using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SchoolChallenge.Contracts;
using SchoolChallenge.Repository.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolChallenge.Services.Repository
{
    public interface IDataRepository
    {
        Task<QueryResult<Student>> GetAllStudentsAsync(string school, RepositoryContinationToken continuationToken);
        Task<QueryResult<Teacher>> GetAllTeachersAsync(string school, RepositoryContinationToken continuationToken);
        Task UpsertStudentAsync(Student student);
        Task UpsertTeacherAsync(Teacher teacher);
        Task DeleteStudentAsync(Student student);
        Task DeleteTeacherAsync(Teacher teacher);        
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

        public async Task<QueryResult<Student>> GetAllStudentsAsync(string school, RepositoryContinationToken continuationToken = null)
        {
            var table = GetStorageAccount().
                CreateCloudTableClient().
                GetTableReference(_settings.StudentTable);

            var query = new TableQuery<StudentEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, school));

            var ct = continuationToken.ToRepositoryImplementation();

            var results = await table.ExecuteQuerySegmentedAsync(query, ct);

            return new QueryResult<Student>
            {
                Results = results.Results.Select(res => res.ToStudent()).ToList(),
                ContinuationToken = new RepositoryContinationToken { Value = results.ContinuationToken }
            };
        }

        public async Task<QueryResult<Teacher>> GetAllTeachersAsync(string school, RepositoryContinationToken continuationToken = null)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_settings.TeacherTable);

            var query = new TableQuery<TeacherEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, school));

            var ct = continuationToken.ToRepositoryImplementation();

            var results = await table.ExecuteQuerySegmentedAsync(query, ct);

            return new QueryResult<Teacher>
            {
                Results = results.Results.Select(res => res.ToTeacher()).ToList(),
                ContinuationToken = new RepositoryContinationToken { Value = results.ContinuationToken }
            };
        }

        public async Task UpsertStudentAsync(Student student)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_settings.StudentTable);

            var entity = student.ToRepositoryEntity();

            var operation = TableOperation.InsertOrReplace(entity);
            
            await table.ExecuteAsync(operation);
        }

        public async Task UpsertTeacherAsync(Teacher teacher)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_settings.TeacherTable);

            var entity = teacher.ToRepositoryEntity();

            var operation = TableOperation.InsertOrReplace(entity);

            await table.ExecuteAsync(operation);
        }

        public async Task DeleteStudentAsync(Student student)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_settings.StudentTable);

            var entity = student.ToRepositoryEntity();
            entity.ETag = "*";

            var operation = TableOperation.Delete(entity);

            await table.ExecuteAsync(operation);
        }

        public async Task DeleteTeacherAsync(Teacher teacher)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_settings.TeacherTable);

            var entity = teacher.ToRepositoryEntity();
            entity.ETag = "*";

            var operation = TableOperation.Delete(entity);

            await table.ExecuteAsync(operation);
        }
    }
}
