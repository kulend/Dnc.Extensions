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

        public override string FormatQuerySql<TEntity>(List<string> searchFields, List<string> whereFields, string order, bool isOne)
        {
            var sql = new StringBuilder("SELECT ");
            if (searchFields != null && searchFields.Any())
            {
                sql.Append(string.Join(",", searchFields.Select(p => QuoteFiled(p))));
            }
            else
            {
                sql.Append("*");
            }
            sql.Append(" FROM " + FormatTableName<TEntity>());
            if (whereFields != null && whereFields.Any())
            {
                sql.Append(" WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteFiled(p) + "=@" + p)));
            }

            //order by
            sql.Append(FormatOrderSql(order));

            if (isOne)
            {
                sql.Append(" LIMIT 1");
            }
            return sql.ToString();
        }

        public override string FormatQueryPageSql<TEntity>(int page, int rows, List<string> searchFields, List<string> whereFields, string order)
        {
            var sql = new StringBuilder("SELECT ");
            if (searchFields != null && searchFields.Any())
            {
                sql.Append(string.Join(",", searchFields.Select(p => QuoteFiled(p))));
            }
            else
            {
                sql.Append("*");
            }
            sql.Append(" FROM " + FormatTableName<TEntity>());
            if (whereFields != null && whereFields.Any())
            {
                sql.Append(" WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteFiled(p) + "=@" + p)));
            }

            //order by
            sql.Append(FormatOrderSql(order));

            sql.Append($" LIMIT {(page - 1) * rows} {rows}");

            return sql.ToString();
        }
    }
}
