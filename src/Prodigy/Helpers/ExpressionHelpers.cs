using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Prodigy.Helpers
{
    public static class ExpressionHelpers
    {
        public static Expression<Func<TTo, bool>> ConvertExpression<TFrom, TTo>(this Expression<Func<TFrom, bool>> from)
        {
            return ConvertImpl<Func<TFrom, bool>, Func<TTo, bool>>(from);
        }

        public static Expression<Func<TTo, object>> ConvertExpression<TFrom, TTo>(this Expression<Func<TFrom, object>> from)
        {
            return ConvertImpl<Func<TFrom, object>, Func<TTo, object>>(from);
        }

        private static Expression<TTo> ConvertImpl<TFrom, TTo>(Expression<TFrom> from)
          where TFrom : class
          where TTo : class
        {
            // figure out which types are different in the function-signature
            var fromTypes = from.Type.GetGenericArguments();
            var toTypes = typeof(TTo).GetGenericArguments();
            if (fromTypes.Length != toTypes.Length)
                throw new NotSupportedException("Incompatible lambda function-type signatures");
            var typeMap = new Dictionary<Type, Type>();
            for (int i = 0; i < fromTypes.Length; i++)
            {
                if (fromTypes[i] != toTypes[i])
                    typeMap[fromTypes[i]] = toTypes[i];
            }

            // re-map all parameters that involve different types
            var parameterMap = new Dictionary<Expression, Expression>();
            var newParams = new ParameterExpression[from.Parameters.Count];
            for (int i = 0; i < newParams.Length; i++)
            {
                Type newType;
                if (typeMap.TryGetValue(from.Parameters[i].Type, out newType))
                {
                    parameterMap[from.Parameters[i]] = newParams[i] =
                        Expression.Parameter(newType, from.Parameters[i].Name);
                }
                else
                {
                    newParams[i] = from.Parameters[i];
                }
            }

            // rebuild the lambda
            var body = new TypeConversionVisitor(parameterMap).Visit(from.Body);
            return Expression.Lambda<TTo>(body, newParams);
        }
    }

    class TypeConversionVisitor : ExpressionVisitor
    {
        private readonly Dictionary<Expression, Expression> parameterMap;

        public TypeConversionVisitor(
            Dictionary<Expression, Expression> parameterMap)
        {
            this.parameterMap = parameterMap;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            // re-map the parameter
            Expression found;
            if (!parameterMap.TryGetValue(node, out found))
                found = base.VisitParameter(node);
            return found;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            // re-perform any member-binding
            var expr = Visit(node.Expression);
            if (expr != null && expr.Type != node.Type)
            {
                var newMember = expr.Type.GetMember(node.Member?.Name).SingleOrDefault();
                if (newMember != null)
                    return Expression.MakeMemberAccess(expr, newMember);
            }
            return base.VisitMember(node);
        }
    }
}
