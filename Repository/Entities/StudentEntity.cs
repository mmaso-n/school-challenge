using Microsoft.WindowsAzure.Storage.Table;

namespace SchoolChallenge.Repository.Entities
{
    // PartitionKey: School
    // RowKey: StudentId
    public class StudentEntity : TableEntity
    {
        public string Number { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? TeacherId { get; set; }

        public bool HasScholarship { get; set; }
    }
}
