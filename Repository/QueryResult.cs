using System.Collections.Generic;

namespace SchoolChallenge.Repository
{
    /// <summary>
    /// Query results from queries ran against this repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryResult<T>
    {
        public IList<T> Results { get; set; }
        public RepositoryContinationToken ContinuationToken { get; set; }
    }
}
