using System;
using System.Linq.Expressions;

namespace Dnc.Extensions.Dapper.Builders
{
    public static class ConditionBuilderExtensions
    {
        public static ConditionBuilder Equal<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();

            return builder.Expression(builder.FormatFiled(t1, n1), "=", value);
        }

        public static ConditionBuilder NotEqual<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();

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
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();

            return builder.Expression(builder.FormatFiled(t1, n1), ">", value);
        }

        public static ConditionBuilder GreaterOrEqual<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();

            return builder.Expression(builder.FormatFiled(t1, n1), ">=", value);
        }

        public static ConditionBuilder Less<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();

            return builder.Expression(builder.FormatFiled(t1, n1), "<", value);
        }

        public static ConditionBuilder LessOrEqual<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field, object value)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();

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

        public static ConditionBuilder IsNull<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();
            return builder.IsNull(builder.FormatFiled(t1, n1));
        }

        public static ConditionBuilder IsNullOrEmpty<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();
            return builder.IsNullOrEmpty(builder.FormatFiled(t1, n1));
        }

        public static ConditionBuilder IsNotNull<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();
            return builder.IsNotNull(builder.FormatFiled(t1, n1));
        }

        public static ConditionBuilder IsNotNullOrEmpty<TEntity>(this ConditionBuilder builder, Expression<Func<TEntity, object>> field)
        {
            var t1 = ConditionBuilder.FormatTableAliasKey<TEntity>();
            var n1 = field.GetPropertyName();
            return builder.IsNotNullOrEmpty(builder.FormatFiled(t1, n1));
        }
    }
}
