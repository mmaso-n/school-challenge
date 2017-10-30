using Microsoft.WindowsAzure.Storage.Table;

namespace SchoolChallenge.Repository.Data
{
    // PartitionKey: School
    // RowKey: StudentId
    public class Student : TableEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? TeacherId { get; set; }

        public bool HasScholarship { get; set; }
    }
}
