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
        protected StringBuilder sb = new StringBuilder();
        protected StringBuilder sbField = new StringBuilder();

        protected int _page = 1;
        protected int _rows = 10;

        private IDictionary<string, string> dictTable = new Dictionary<string, string>();

        public QueryBuilder Select<TEntity>() where TEntity : class
        {
            var key = FormatTableAliasKey<TEntity>();
            sbField.Append($"{key}.*");
            return this;
        }

        public QueryBuilder Select<TEntity>(Expression<Func<TEntity, object>> fieldExpression) where TEntity : class
        {
            var key = FormatTableAliasKey<TEntity>();

            if (fieldExpression.Body.Type.Name.Contains("AnonymousType"))
            {
                var props = fieldExpression.Body.Type.GetProperties();
                sbField.Append(string.Join(",", props.Select(x => FormatFiled(key, x.Name))));
            }
            return this;
        }

        public QueryBuilder Concat<TEntity>()
        {
            sbField.Append(",");
            var key = FormatTableAliasKey<TEntity>();

            sbField.Append($"{key}.*");
            return this;
        }

        public QueryBuilder Concat<TEntity>(Expression<Func<TEntity, object>> fieldExpression) where TEntity : class
        {
            sbField.Append(",");
            var key = FormatTableAliasKey<TEntity>();

            if (fieldExpression.Body.Type.Name.Contains("AnonymousType"))
            {
                var props = fieldExpression.Body.Type.GetProperties();
                sbField.Append(string.Join(",", props.Select(x => FormatFiled(key, x.Name))));
            }
            return this;
        }

        public QueryBuilder From<TEntity>(string @as = null) where TEntity : class
        {
            if (string.IsNullOrEmpty(@as))
            {
                @as = "m";
            }
            var key = FormatTableAliasKey<TEntity>();
            dictTable.Add(key, @as);

            sb.Append($" from {_dialect.FormatTableName<TEntity>()} as {@as}");
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

            sb.Append($" left join {_dialect.FormatTableName<TEntity>()} as {@as}");
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

            sb.Append($" inner join {_dialect.FormatTableName<TEntity>()} as {@as}");
            return this;
        }

        public QueryBuilder On(ConditionBuilder builder)
        {
            var on = builder.Build();
            param.TryConcat(on.param);
            sb.Append(" on " + on.sql);
            return this;
        }

        public QueryBuilder Where(ConditionBuilder builder)
        {
            var where = builder.Build();
            param.TryConcat(where.param);
            sb.Append(" where " + where.sql);
            return this;
        }

        public QueryBuilder Sort(object sort)
        {
            if (sort is string s && !string.IsNullOrEmpty(s))
            {
                sb.Append($" order by {s}");
            }
            
            return this;
        }

        public QueryBuilder OrderBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            sb.Append($" order by ");
            AppendOrderBy<TEntity>(fields, false);
            return this;
        }

        public QueryBuilder OrderByDesc<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            sb.Append($" order by ");
            AppendOrderBy<TEntity>(fields, true);
            return this;
        }

        public QueryBuilder ThenBy<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                return this;
            }
            sb.Append($",");
            AppendOrderBy<TEntity>(fields, false);
            return this;
        }

        public QueryBuilder ThenByDesc<TEntity>(params Expression<Func<TEntity, object>>[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                return this;
            }
            sb.Append($",");
            AppendOrderBy<TEntity>(fields, true);
            return this;
        }

        public QueryBuilder Limit(int page = 1, int rows = 10)
        {
            _page = page;
            _rows = rows;
            return this;
        }

        public (int page, int rows) GetLimit()
        {
            return (_page, _rows);
        }

        private void AppendOrderBy<TEntity>(Expression<Func<TEntity, object>>[] fields, bool isDesc = false)
        {
            var key = FormatTableAliasKey<TEntity>();
            List<string> items = new List<string>();
            foreach (var item in fields)
            {
                var name = item.GetPropertyName();
                items.Add(FormatFiled(key, name) + (isDesc ? " desc" : ""));
            }
            sb.Append(string.Join(",", items));
        }

        public (string sql, string pageSql, string countSql, Dictionary<string, object> param) Build()
        {
            //替换所以表别名
            var text = "select " + sbField.ToString() + sb.ToString();
            var textCount = "select count(1)" + sb.ToString();
            foreach (var item in dictTable)
            {
                text = text.Replace(item.Key, item.Value);
                textCount = textCount.Replace(item.Key, item.Value);
            }

            var pageSql = _dialect.FormatQueryPageSql(_page, _rows, text);

            return (text, pageSql, textCount, param);
        }
    }
}
