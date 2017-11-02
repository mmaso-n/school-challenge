using System.Collections.Generic;

namespace SchoolChallenge.Repository
{
    public class QueryResult<T>
    {
        public IList<T> Results { get; set; }
        public RepositoryContinationToken ContinuationToken { get; set; }
    }
}
