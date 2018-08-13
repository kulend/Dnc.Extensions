using Dnc.Extensions.Dapper.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dnc.Extensions.Dapper.Builders
{
    public class QueryBuilder : BaseBuilder
    {
        private int tableIndex = 0;
        protected StringBuilder sql = new StringBuilder();

        private IDictionary<string, string> dictTable = new Dictionary<string, string>();

        public QueryBuilder Select<TEntity>() where TEntity : class
        {
            sql.Append("select ");
            var key = FormatTableAliasKey<TEntity>();

            sql.Append($"{key}.*");
            return this;
        }

        public QueryBuilder Select<TEntity>(Expression<Func<TEntity, object>> fieldExpression) where TEntity : class
        {
            sql.Append("select ");
            var key = FormatTableAliasKey<TEntity>();

            if (fieldExpression.Body.Type.Name.Contains("AnonymousType"))
            {
                var props = fieldExpression.Body.Type.GetProperties();
                sql.Append(string.Join(",", props.Select(x => FormatFiled(key, x.Name))));
            }
            return this;
        }

        public QueryBuilder Concat<TEntity>()
        {
            sql.Append(",");
            var key = FormatTableAliasKey<TEntity>();

            sql.Append($"{key}.*");
            return this;
        }

        public QueryBuilder Concat<TEntity>(Expression<Func<TEntity, object>> fieldExpression) where TEntity : class
        {
            sql.Append(",");
            var key = FormatTableAliasKey<TEntity>();

            if (fieldExpression.Body.Type.Name.Contains("AnonymousType"))
            {
                var props = fieldExpression.Body.Type.GetProperties();
                sql.Append(string.Join(",", props.Select(x => FormatFiled(key, x.Name))));
            }
            return this;
        }

        public QueryBuilder From<TEntity>(string @as = null) where TEntity : class
        {
            if (string.IsNullOrEmpty(@as))
            {
                @as = "t" + (tableIndex++).ToString();
            }
            var key = FormatTableAliasKey<TEntity>();
            dictTable.Add(key, @as);

            sql.Append($" from {_dialect.FormatTableName<TEntity>()} as {@as}");
            return this;
        }

        public QueryBuilder LeftJoin<TEntity>(string @as = null) where TEntity : class
        {
            if (string.IsNullOrEmpty(@as))
            {
                @as = "t" + (tableIndex++).ToString();
            }

            var key = FormatTableAliasKey<TEntity>();
            dictTable.Add(key, @as);

            sql.Append($" left join {_dialect.FormatTableName<TEntity>()} as {@as}");
            return this;
        }

        public QueryBuilder InnerJoin<TEntity>(string @as = null) where TEntity : class
        {
            if (string.IsNullOrEmpty(@as))
            {
                @as = "t" + (tableIndex++).ToString();
            }

            var key = FormatTableAliasKey<TEntity>();
            dictTable.Add(key, @as);

            sql.Append($" inner join {_dialect.FormatTableName<TEntity>()} as {@as}");
            return this;
        }

        public QueryBuilder On(ConditionBuilder builder)
        {
            var on = builder.Build();
            param.TryConcat(on.param);
            sql.Append(" on " + on.sql);
            return this;
        }

        public QueryBuilder Where(ConditionBuilder builder)
        {
            var where = builder.Build();
            param.TryConcat(where.param);
            sql.Append(" where " + where.sql);
            return this;
        }

        public QueryBuilder Where<TEntity>(DapperSearch<TEntity> search)
        {
            var where = ConditionBuilder.FromSearch<TEntity>(search).Build();
            param.TryConcat(where.param);
            sql.Append(" where " + where.sql);
            return this;
        }

        public QueryBuilder OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            sql.Append($" order by ");
            AppendOrderBy<TEntity>(fields, false);
            return this;
        }

        public QueryBuilder OrderByDesc<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            sql.Append($" order by ");
            AppendOrderBy<TEntity>(fields, true);
            return this;
        }

        public QueryBuilder ThenBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                return this;
            }
            sql.Append($",");
            AppendOrderBy<TEntity>(fields, false);
            return this;
        }

        public QueryBuilder ThenByDesc<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                return this;
            }
            sql.Append($",");
            AppendOrderBy<TEntity>(fields, true);
            return this;
        }

        private void AppendOrderBy<TEntity>(Expression<Func<TEntity, object>>[] fields, bool isDesc = false)
        {
            var key = FormatTableAliasKey<TEntity>();
            List<string> items = new List<string>();
            foreach (var item in fields)
            {
                var name = ExpressionHelper.GetPropertyName(item);
                items.Add(FormatFiled(key, name) + (isDesc ? " desc" : ""));
            }
            sql.Append(string.Join(",", items));
        }

        public override (string sql, Dictionary<string, object> param) Build()
        {
            //替换所以表别名
            var text = sql.ToString();
            foreach (var item in dictTable)
            {
                text = text.Replace(item.Key, item.Value);
            }
            return (text, param);
        }
    }
}
