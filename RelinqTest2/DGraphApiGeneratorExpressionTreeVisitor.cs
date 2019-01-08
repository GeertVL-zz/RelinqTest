using System;
using System.Linq.Expressions;
using System.Text;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace RelinqTest2
{
    public class DGraphApiGeneratorExpressionTreeVisitor : ThrowingExpressionVisitor
    {
        private readonly StringBuilder _expression = new StringBuilder();

        public static string GetDGraphExpression(Expression expression)
        {
            var visitor = new DGraphApiGeneratorExpressionTreeVisitor();
            visitor.Visit(expression);
            return visitor.GetExpression();
        }

        public string GetExpression()
        {
            return _expression.ToString();
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            return expression;
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            _expression.Append("(");
            Visit(expression.Left);
            if (expression.NodeType == ExpressionType.Equal)
            {
                _expression.Append(" GELIJK ");
            }

            Visit(expression.Right);
            _expression.Append(")");

            return expression;
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            if (expression.Member.Name == "Length")
            {
                var parts = ((MemberExpression)expression.Expression).Member.Name.Split('.');
                _expression.AppendFormat("length({0})", parts[parts.Length - 1]);
            }
            else
            {
                Visit(expression.Expression);
                _expression.AppendFormat("{0}", expression.Member.Name);
            }

            return expression;
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            var value = expression.Value.ToString();
            _expression.AppendFormat("[|{0}|]", value);

            return expression;
        }

        protected override Expression VisitNew(NewExpression expression)
        {
            _expression.Append("{");
            int i = 0;
            foreach (var arg in expression.Arguments)
            {
                _expression.AppendFormat("{0} | ", expression.Members[i].Name);
                Visit(arg);
                i++;
            }

            _expression.Append("}");

            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            _expression.Append($"roep: {expression.Method.Name}");

            return expression;
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            return new NotSupportedException("NOT SUPPORTED");
        }

    }
}