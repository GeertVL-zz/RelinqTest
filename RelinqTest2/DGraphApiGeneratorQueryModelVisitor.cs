using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace RelinqTest2
{
    public class DGraphApiGeneratorQueryModelVisitor : QueryModelVisitorBase
    {
        private readonly QueryPartsAggregator _queryParts = new QueryPartsAggregator();

        public QueryPartsAggregator QueryParts
            => _queryParts;

        public static CommandData GenerateApiQuery(QueryModel queryModel)
        {
            var visitor = new DGraphApiGeneratorQueryModelVisitor();
            visitor.VisitQueryModel(queryModel);
            return visitor.GetApiCommand();
        }

        public CommandData GetApiCommand()
        {
            return new CommandData(_queryParts);
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            queryModel.SelectClause.Accept(this, queryModel);
            queryModel.MainFromClause.Accept(this, queryModel);
            VisitBodyClauses(queryModel.BodyClauses, queryModel);
            VisitResultOperators(queryModel.ResultOperators, queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (resultOperator is FirstResultOperator)
            {
                _queryParts.Take = 1;
                return;
            }

            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            _queryParts.AddFromPart(fromClause);

            base.VisitMainFromClause(fromClause, queryModel);

            var subQueryExpression = fromClause.FromExpression as SubQueryExpression;
            if (subQueryExpression == null)
                return;

            VisitQueryModel(subQueryExpression.QueryModel);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            if (String.IsNullOrEmpty(_queryParts.SelectPart))
            {
                _queryParts.SelectPart = GetApiExpression(selectClause.Selector);
            }

            base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            _queryParts.AddWherePart(GetApiExpression(whereClause.Predicate));

            base.VisitWhereClause(whereClause, queryModel, index);
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            if (orderByClause.Orderings.Any())
            {
                _queryParts.AddOrderByPart(GetApiExpression(orderByClause.Orderings[0].Expression), orderByClause.Orderings[0].OrderingDirection == OrderingDirection.Desc);
            }

            base.VisitOrderByClause(orderByClause, queryModel, index);
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            _queryParts.AddFromPart(joinClause);
            _queryParts.AddWherePart(
                "({0} = {1})",
                GetApiExpression(joinClause.OuterKeySelector),
                GetApiExpression(joinClause.InnerKeySelector));

            base.VisitJoinClause(joinClause, queryModel, index);
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            _queryParts.AddFromPart(fromClause);

            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException();
        }

        private string GetApiExpression(Expression expression)
        {
            return DGraphApiGeneratorExpressionTreeVisitor.GetDGraphExpression(expression);
        }
    }
}