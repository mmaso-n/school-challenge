using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SchoolChallenge.Contracts;
using SchoolChallenge.Repository.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolChallenge.Repository
{
    public interface IDataRepository
    {
        Task<QueryResult<Student>> GetAllStudentsAsync(string school, RepositoryContinationToken continuationToken);
        Task<QueryResult<Teacher>> GetAllTeachersAsync(string school, RepositoryContinationToken continuationToken);
        Task<QueryResult<Student>> SearchStudentsAsync(string school, int? studentId, string studentNumber, string firstName = null, string lastName = null, int? teacherId = null, bool? hasScholarship = default(bool?), RepositoryContinationToken continuationToken = null);
        Task<QueryResult<Teacher>> SearchTeachersAsync(string school, int? teacherId, string lastName = null, string firstName = null, RepositoryContinationToken continuationToken = null);
        Task UpsertStudentAsync(Student student);
        Task UpsertTeacherAsync(Teacher teacher);
        Task DeleteStudentAsync(Student student);
        Task DeleteTeacherAsync(Teacher teacher);        
    }

    public class DataRepository : IDataRepository
    {
        private CloudStorageAccount _cloudStorageAccount;

        private readonly string _connectionString;
        private readonly string _teacherTableName;        
        private readonly string _studentTableName;

        public DataRepository(string connectionString, string studentTableName, string teacherTableName)
        {
            _connectionString = connectionString;
            _studentTableName = studentTableName;
            _teacherTableName = teacherTableName;
        }
        
        private CloudStorageAccount GetStorageAccount()
        {
            if (_cloudStorageAccount == null)
                _cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);

            return _cloudStorageAccount;
        }

        public async Task<QueryResult<Student>> GetAllStudentsAsync(string school, RepositoryContinationToken continuationToken = null)
        {
            var table = GetStorageAccount().
                CreateCloudTableClient().
                GetTableReference(_studentTableName);

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
                .GetTableReference(_teacherTableName);

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
                .GetTableReference(_studentTableName);

            var entity = student.ToRepositoryEntity();

            var operation = TableOperation.InsertOrReplace(entity);
            
            await table.ExecuteAsync(operation);
        }

        public async Task UpsertTeacherAsync(Teacher teacher)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_teacherTableName);

            var entity = teacher.ToRepositoryEntity();

            var operation = TableOperation.InsertOrReplace(entity);

            await table.ExecuteAsync(operation);
        }

        public async Task DeleteStudentAsync(Student student)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_studentTableName);

            var entity = student.ToRepositoryEntity();
            entity.ETag = "*";

            var operation = TableOperation.Delete(entity);

            await table.ExecuteAsync(operation);
        }

        public async Task DeleteTeacherAsync(Teacher teacher)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_teacherTableName);

            var entity = teacher.ToRepositoryEntity();
            entity.ETag = "*";

            var operation = TableOperation.Delete(entity);

            await table.ExecuteAsync(operation);
        }

        public async Task<QueryResult<Student>> SearchStudentsAsync(string school, int? studentId, string studentNumber = null,
            string firstName = null, string lastName = null, int? teacherId = null, bool? hasScholarship = default(bool?), 
            RepositoryContinationToken continuationToken = null)
        {
            var table = GetStorageAccount().
                CreateCloudTableClient().
                GetTableReference(_studentTableName);

            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, school.Trim());
            
            if (studentId.HasValue)
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, studentId.Value.ToString()));

            if (!string.IsNullOrWhiteSpace(studentNumber))
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition(nameof(StudentEntity.Number), QueryComparisons.Equal, studentNumber.Trim()));

            if (!string.IsNullOrWhiteSpace(firstName))
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition(nameof(StudentEntity.FirstName), QueryComparisons.Equal, firstName.Trim()));

            if (!string.IsNullOrWhiteSpace(lastName))
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition(nameof(StudentEntity.LastName), QueryComparisons.Equal, lastName.Trim()));

            if (teacherId.HasValue)
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterConditionForInt(nameof(StudentEntity.TeacherId), QueryComparisons.Equal, teacherId.Value));

            if (hasScholarship.HasValue)
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterConditionForBool(nameof(StudentEntity.HasScholarship), QueryComparisons.Equal, hasScholarship.Value));

            var query = new TableQuery<StudentEntity>().Where(filter.ToString());

            var ct = continuationToken.ToRepositoryImplementation();

            var results = await table.ExecuteQuerySegmentedAsync(query, ct);

            return new QueryResult<Student>
            {
                Results = results.Results.Select(res => res.ToStudent()).ToList(),
                ContinuationToken = new RepositoryContinationToken { Value = results.ContinuationToken }
            };
        }

        public async Task<QueryResult<Teacher>> SearchTeachersAsync(string school, int? teacherId, 
            string lastName = null, string firstName = null, RepositoryContinationToken continuationToken = null)
        {
            var table = GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(_teacherTableName);

            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, school);

            if (teacherId.HasValue)
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, teacherId.Value.ToString().Trim()));

            if (!string.IsNullOrWhiteSpace(firstName))
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition(nameof(TeacherEntity.FirstName), QueryComparisons.Equal, firstName.Trim()));

            if (!string.IsNullOrWhiteSpace(lastName))
                filter = TableQuery.CombineFilters(filter, TableOperators.And, TableQuery.GenerateFilterCondition(nameof(TeacherEntity.LastName), QueryComparisons.Equal, lastName.Trim()));

            var query = new TableQuery<TeacherEntity>().Where(filter.ToString());

            var ct = continuationToken.ToRepositoryImplementation();

            var results = await table.ExecuteQuerySegmentedAsync(query, ct);

            return new QueryResult<Teacher>
            {
                Results = results.Results.Select(res => res.ToTeacher()).ToList(),
                ContinuationToken = new RepositoryContinationToken { Value = results.ContinuationToken }
            };
        }
    }
}
