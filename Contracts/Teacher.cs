namespace Contracts
{
    /// <summary>
    /// Represents a school teach
    /// </summary>
    public class Teacher
    {
        /// <summary>
        /// The school in which this teacher resides
        /// </summary>
        public string School { get; set; }

        /// <summary>
        /// Unique Teacher Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Teacher First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Teacher Last Name
        /// </summary>
        public string LastName { get; set; }
    }
}
