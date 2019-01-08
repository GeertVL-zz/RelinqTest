using System.Collections.Generic;

namespace RelinqTest2
{
    public class DGraphQuery
    {
        private readonly QueryPartsAggregator _queryParts;

        public DGraphQuery(QueryPartsAggregator queryParts)
        {
            _queryParts = queryParts;
        }

        // Here you execute your query to the DGraph server with Json
        public IEnumerable<T> Enumerable<T>()
        {
            return null;
        }
    }
}
