using SchoolChallenge.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Tests
{
    static class TestHelpers
    {
        internal static Task<QueryResult<T>> GetMockQueryResult<T>()
        {
            return Task.FromResult(new QueryResult<T> { Results = new List<T>(), ContinuationToken = new RepositoryContinationToken() });
        }
    }
}
