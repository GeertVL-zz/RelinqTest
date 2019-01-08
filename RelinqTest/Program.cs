using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionVisitors;
using Remotion.Linq.Parsing.Structure;

namespace RelinqTest
{
    public class SampleDataSourceItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    internal class SampleQueryable<T> : QueryableBase<T>
    {
        public SampleQueryable(IQueryParser queryParser, IQueryExecutor queryExecutor)
            : base(new DefaultQueryProvider(typeof(SampleQueryable<>), queryParser, queryExecutor))
        {
        }

        public SampleQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }

    internal class SampleQueryExecutor : IQueryExecutor
    {
        public SampleDataSourceItem Current { get; private set; }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var sequence = ExecuteCollection<T>(queryModel);
            return returnDefaultWhenEmpty ? sequence.SingleOrDefault() : sequence.Single();
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        { 
            // Create an expression that returns the current item when invoked
            Expression currentItemExpression = Expression.Property(Expression.Constant(this), "Current");

            // Now replace references like the "i" in "select i" that refers to the "i" in "from i in items"
            var mapping = new QuerySourceMapping();
            mapping.AddMapping(queryModel.MainFromClause, currentItemExpression);
            queryModel.TransformExpressions(e =>
                ReferenceReplacingExpressionVisitor.ReplaceClauseReferences(e, mapping, true));

            // Create a lambda that takes our SampleDataSourceItem and passes it through the select clause
            // to produce a type of T. (T may be SampleDataSourceItem, in which case this is an identity function).
            var currentItemProperty = Expression.Parameter(typeof(SampleDataSourceItem));
            var projection =
                Expression.Lambda<Func<SampleDataSourceItem, T>>(queryModel.SelectClause.Selector, currentItemProperty);
            var projector = projection.Compile();

            for (var i = 0; i < 10; i++)
            {
                Current = new SampleDataSourceItem
                {
                    Name = "Name " + i,
                    Description = "This describes the item in position " + i
                };

                yield return projector(Current);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var queryParser = QueryParser.CreateDefault();
            var items = new SampleQueryable<SampleDataSourceItem>(queryParser, new SampleQueryExecutor());

            var results = from i in items select i;
            var list = results.ToList();

            if (list.Count == 10)
            {

            }
        }
    }
}
