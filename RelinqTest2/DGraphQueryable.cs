using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Clauses.ExpressionVisitors;

namespace RelinqTest2
{
    public class DGraphQueryable<T> : QueryableBase<T>
    {
        public DGraphQueryable(IQueryParser queryParser, IQueryExecutor executor)
            : base(queryParser, executor)
        {
        }

        public DGraphQueryable(IQueryProvider provider)
            : base(provider)
        {
        }

        public DGraphQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

        public DGraphQueryable()
            : base(QueryParser.CreateDefault(), CreateExecutor())
        {
            
        }

        private static IQueryExecutor CreateExecutor()
        {
            return new DGraphQueryExecutor();
        }
    }
}