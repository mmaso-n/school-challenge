using Microsoft.WindowsAzure.Storage.Table;

namespace SchoolChallenge.Repository.Data
{
    // PartitionKey: School
    // RowKey: TeacherId
    public class Teacher : TableEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
