using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dnc.Extensions.Dapper.Sql;
using System.Reflection;
using Dnc.Extensions.Dapper.SqlDialect;

namespace Dnc.Extensions.Dapper.QueryBuilder
{
    public class DapperQueryBuilder : BaseBuilder
    {
        protected StringBuilder sql = new StringBuilder();

        private IDictionary<string, List<string>> dictField = new Dictionary<string, List<string>>();
        private IDictionary<string, string> dictTable = new Dictionary<string, string>();

        public DapperQueryBuilder Select<TEntity>(Expression<Func<TEntity, object>> fieldExpression) where TEntity : class
        {
            var key = typeof(TEntity).FullName;
            if (!dictField.TryGetValue(key, out List<string> fields))
            {
                fields = new List<string>();
            }

            if (fieldExpression.Body.Type.Name.Contains("AnonymousType"))
            {
                var props = fieldExpression.Body.Type.GetProperties();
                fields.AddRange(props.Select(x => x.Name));
            }
            dictField.Add(key, fields);
            return this;
        }

        public DapperQueryBuilder From<TEntity>(string @as = null) where TEntity : class
        {
            var key = typeof(TEntity).FullName;
            dictTable.Add(key, @as);

            sql.Append($" FROM {_dialect.FormatTableName<TEntity>()} ");
            if (!string.IsNullOrEmpty(@as))
            {
                sql.Append($"AS {@as} ");
            }
            return this;
        }

        public DapperQueryBuilder LeftJoin<TEntity>(string @as = null) where TEntity : class
        {
            var key = typeof(TEntity).FullName;
            dictTable.Add(key, @as);

            sql.Append($" LEFT JOIN {_dialect.FormatTableName<TEntity>()} ");
            if (!string.IsNullOrEmpty(@as))
            {
                sql.Append($"AS {@as} ");
            }
            return this;
        }

        public DapperQueryBuilder InnerJoin<TEntity>(string @as = null) where TEntity : class
        {
            var key = typeof(TEntity).FullName;
            dictTable.Add(key, @as);

            sql.Append($" INNER JOIN {_dialect.FormatTableName<TEntity>()} ");
            if (!string.IsNullOrEmpty(@as))
            {
                sql.Append($"AS {@as} ");
            }
            return this;
        }

        public DapperQueryBuilder On(LogicConditionBuilder builder)
        {
            var on = builder.Build();
            param.TryConcat(on.param);
            sql.Append(" On " + on.sql);
            return this;
        }

        public DapperQueryBuilder Where(LogicConditionBuilder builder)
        {
            var where = builder.Build();
            param.TryConcat(where.param);
            sql.Append(" WHERE " + where.sql);
            return this;
        }

        public DapperQueryBuilder OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            sql.Append($" ORDER BY ");
            var t = typeof(TEntity).FullName;
            List<string> items = new List<string>();
            foreach (var item in fields)
            {
                var name = ExpressionHelper.GetPropertyName(item);
                items.Add(FormatFiled(t, name));
            }
            sql.Append(string.Join(",", items));
            return this;
        }

        public DapperQueryBuilder OrderByDesc<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            sql.Append($" ORDER BY ");
            var t = typeof(TEntity).FullName;
            List<string> items = new List<string>();
            foreach (var item in fields)
            {
                var name = ExpressionHelper.GetPropertyName(item);
                items.Add(FormatFiled(t, name) + " DESC");
            }
            sql.Append(string.Join(",", items));
            return this;
        }

        public DapperQueryBuilder ThenBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            sql.Append($",");
            var t = typeof(TEntity).FullName;
            List<string> items = new List<string>();
            foreach (var item in fields)
            {
                var name = ExpressionHelper.GetPropertyName(item);
                items.Add(FormatFiled(t, name));
            }
            sql.Append(string.Join(",", items));
            return this;
        }

        public DapperQueryBuilder ThenByDesc<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            sql.Append($",");
            var t = typeof(TEntity).FullName;
            List<string> items = new List<string>();
            foreach (var item in fields)
            {
                var name = ExpressionHelper.GetPropertyName(item);
                items.Add(FormatFiled(t, name) + " DESC");
            }
            sql.Append(string.Join(",", items));
            return this;
        }

        public override (string sql, Dictionary<string, object> param) Build()
        {
            return (sql.ToString(), param);
        }
    }
}
