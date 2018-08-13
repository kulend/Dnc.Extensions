using Dnc.Extensions.Dapper.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dnc.Extensions.Dapper.Builders
{
    public static class ConditionBuilderExtensions
    {
        public static ConditionBuilder Equal<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = builder.FormatTableAliasKey<TEntity>();
            var n1 = ExpressionHelper.GetPropertyName(field);

            return builder.Expression(builder.FormatFiled(t1, n1), "=", value);
        }

        public static ConditionBuilder NotEqual<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = builder.FormatTableAliasKey<TEntity>();
            var n1 = ExpressionHelper.GetPropertyName(field);

            return builder.Expression(builder.FormatFiled(t1, n1), "<>", value);
        }

        public static ConditionBuilder Equal<T1, T2>(this ConditionBuilder builder, Expression<Func<T1, object>> field1, Expression<Func<T1, object>> field2)
        {
            return builder.Expression<T1, T2>(field1, "=", field2);
        }

        public static ConditionBuilder NotEqual<T1, T2>(this ConditionBuilder builder, Expression<Func<T1, object>> field1, Expression<Func<T1, object>> field2)
        {
            return builder.Expression<T1, T2>(field1, "<>", field2);
        }

        public static ConditionBuilder Greater<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = builder.FormatTableAliasKey<TEntity>();
            var n1 = ExpressionHelper.GetPropertyName(field);

            return builder.Expression(builder.FormatFiled(t1, n1), ">", value);
        }

        public static ConditionBuilder GreaterOrEqual<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = builder.FormatTableAliasKey<TEntity>();
            var n1 = ExpressionHelper.GetPropertyName(field);

            return builder.Expression(builder.FormatFiled(t1, n1), ">=", value);
        }

        public static ConditionBuilder Less<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = builder.FormatTableAliasKey<TEntity>();
            var n1 = ExpressionHelper.GetPropertyName(field);

            return builder.Expression(builder.FormatFiled(t1, n1), "<", value);
        }

        public static ConditionBuilder LessOrEqual<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = builder.FormatTableAliasKey<TEntity>();
            var n1 = ExpressionHelper.GetPropertyName(field);

            return builder.Expression(builder.FormatFiled(t1, n1), "<=", value);
        }

        public static ConditionBuilder Greater<T1, T2>(this ConditionBuilder builder, Expression<Func<T1, object>> field1, Expression<Func<T1, object>> field2)
        {
            return builder.Expression<T1, T2>(field1, ">", field2);
        }

        public static ConditionBuilder GreaterOrEqual<T1, T2>(this ConditionBuilder builder, Expression<Func<T1, object>> field1, Expression<Func<T1, object>> field2)
        {
            return builder.Expression<T1, T2>(field1, ">=", field2);
        }

        public static ConditionBuilder Less<T1, T2>(this ConditionBuilder builder, Expression<Func<T1, object>> field1, Expression<Func<T1, object>> field2)
        {
            return builder.Expression<T1, T2>(field1, "<", field2);
        }

        public static ConditionBuilder LessOrEqual<T1, T2>(this ConditionBuilder builder, Expression<Func<T1, object>> field1, Expression<Func<T1, object>> field2)
        {
            return builder.Expression<T1, T2>(field1, "<=", field2);
        }


    }
}
