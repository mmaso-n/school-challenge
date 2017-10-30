namespace Contracts
{
    /// <summary>
    /// Represents a school student
    /// </summary>
    public class Student
    {
        /// <summary>
        /// The school in which this student resides
        /// </summary>
        public string School { get; set; }

        /// <summary>
        /// Unique Student Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Student Number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Student First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Student Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Whether or not student has scholarship
        /// </summary>
        public bool HasScholarship { get; set; }

        /// <summary>
        /// Identifer of the student's teacher
        /// </summary>
        public int? TeacherId { get; set; }
    }
}
