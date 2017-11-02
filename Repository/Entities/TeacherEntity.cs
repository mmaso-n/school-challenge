using Microsoft.WindowsAzure.Storage.Table;

namespace SchoolChallenge.Repository.Entities
{
    // PartitionKey: School
    // RowKey: TeacherId
    public class TeacherEntity : TableEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
