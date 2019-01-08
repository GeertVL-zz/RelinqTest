using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;

namespace RelinqTest2
{
    public class DGraphQueryExecutor : IQueryExecutor
    {
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            return ExecuteCollection<T>(queryModel).Single();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty ? ExecuteCollection<T>(queryModel).SingleOrDefault() : ExecuteCollection<T>(queryModel).Single();
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var commandData = DGraphApiGeneratorQueryModelVisitor.GenerateApiQuery(queryModel);
            var query = commandData.CreateQuery();
            return query.Enumerable<T>();
        }
    }
}