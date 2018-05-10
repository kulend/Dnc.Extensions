using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ku.Core.Extensions.Dapper.SqlDialect
{
    public class MySqlDialect : BaseDialect
    {
        public override char QuoteStart
        {
            get { return '`'; }
        }

        public override char QuoteEnd
        {
            get { return '`'; }
        }

        public override string FormatQuerySql<TEntity>(string field, string where, string order, bool isOne)
        {
            var sql = new StringBuilder("SELECT ");
            if (string.IsNullOrEmpty(field))
            {
                field = "*";
            }
            sql.Append(field);
            sql.Append(" FROM " + FormatTableName<TEntity>());

            if (!string.IsNullOrEmpty(where))
            {
                sql.Append(where);
            }

            if (!string.IsNullOrEmpty(order))
            {
                sql.Append(order);
            }

            if (isOne)
            {
                sql.Append(" LIMIT 1");
            }
            return sql.ToString();
        }

        public override string FormatQueryPageSql<TEntity>(int page, int rows, string field, string where, string order)
        {
            var sql = new StringBuilder("SELECT ");
            if (string.IsNullOrEmpty(field))
            {
                field = "*";
            }
            sql.Append(field);
            sql.Append(" FROM " + FormatTableName<TEntity>());

            if (!string.IsNullOrEmpty(where))
            {
                sql.Append(where);
            }

            if (!string.IsNullOrEmpty(order))
            {
                sql.Append(order);
            }

            sql.Append($" LIMIT {(page - 1) * rows},{rows}");

            return sql.ToString();
        }
    }
}
