using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Ku.Core.Extensions.Dapper.SqlDialect
{
    public class BaseDialect : ISqlDialect
    {
        public virtual char QuoteStart
        {
            get { return '"'; }
        }

        public virtual char QuoteEnd
        {
            get { return '"'; }
        }

        public virtual char ParameterPrefix
        {
            get
            {
                return '@';
            }
        }

        public string QuoteFiled(string filed)
        {
            return QuoteStart + filed + QuoteEnd;
        }

        public virtual string FormatInsertSql<TEntity>(List<string> fields) where TEntity : class
        {
            var tableName = FormatTableName<TEntity>();
            var columns = string.Join(",", fields.Select(p => QuoteStart + p + QuoteEnd));
            var values = string.Join(",", fields.Select(p => "@" + p));
            var sql = $"INSERT INTO {tableName} ({columns}) values ({values})";
            return sql;
        }

        public virtual string FormatUpdateSql<TEntity>(List<string> updateFields, List<string> whereFields, string wherePrefix) where TEntity : class
        {
            var tableName = FormatTableName<TEntity>();
            return FormatUpdateSqlSub(tableName, updateFields, whereFields, wherePrefix);
        }

        public virtual string FormatUpdateSql(string tableName, string tableSchema, List<string> updateFields, List<string> whereFields, string wherePrefix)
        {
            tableName = FormatTableName(tableName, tableSchema);
            return FormatUpdateSqlSub(tableName, updateFields, whereFields, wherePrefix);
        }

        private string FormatUpdateSqlSub(string tableName, List<string> updateFields, List<string> whereFields, string wherePrefix)
        {
            var columns = string.Join(",", updateFields.Select(p => QuoteStart + p + QuoteEnd + "=@" + p));
            var where = "";
            if (whereFields.Any())
            {
                where = " WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteStart + p + QuoteEnd + "=@" + wherePrefix + p));
            }
            return $"UPDATE {tableName} SET {columns}{where}";
        }

        public string FormatTableName(string tableName, string tableSchema = null)
        {
            return !string.IsNullOrEmpty(tableSchema)
                ? QuoteStart + tableSchema + QuoteEnd + "." + QuoteStart + tableName + QuoteEnd
                : QuoteStart + tableName + QuoteEnd;
        }

        public string FormatTableName<TEntity>()
        {
            var type = typeof(TEntity);

            var attr = type.GetCustomAttribute<TableAttribute>();
            var tableName = attr != null ? attr.Name : type.Name;
            var tableSchema = attr != null ? attr.Schema : string.Empty;

            return FormatTableName(tableName, tableSchema);
        }

        public virtual string FormatLogicalDeleteRestoreSql<TEntity>(string field, List<string> whereFields = null, string whereSql = null) where TEntity : class
        {
            var where = "";
            if (whereFields != null && whereFields.Any())
            {
                where = "WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteFiled(p) + "=@" + p));
            }
            else if (!string.IsNullOrEmpty(whereSql))
            {
                where = "WHERE " + whereSql;
            }
            else
            {
                where = "WHERE 1<>1 ";
            }

            return $"UPDATE {FormatTableName<TEntity>()} SET {QuoteFiled(field)}=@{field} {where}";
        }

        public virtual string FormatDeleteSql<TEntity>(List<string> whereFields) where TEntity : class
        {
            var where = "WHERE 1<>1 ";
            if (whereFields != null && whereFields.Any())
            {
                where = "WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteFiled(p) + "=@" + p));
            }

            return $"DELETE FROM {FormatTableName<TEntity>()} {where}";
        }

        public virtual string FormatQuerySql<TEntity>(List<string> searchFields, List<string> whereFields, string order, bool isOne) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual string FormatOrderSql(string order)
        {
            if (string.IsNullOrEmpty(order)) return "";

            var sql = "";
            var orderFileds = order.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x=> x.Trim());

            foreach (var item in orderFileds)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    sql += ",";
                }

                var spItem = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                bool desc = (spItem.Length > 1 && spItem[1].Equals("desc", StringComparison.OrdinalIgnoreCase));

                sql += QuoteFiled(spItem[0]) + (desc ? " DESC" : " ASC");
            }

            return " ORDER BY " + sql;
        }

        public virtual string FormatCountSql<TEntity>(List<string> whereFields)
        {
            var sql = new StringBuilder("SELECT COUNT(1)");
            sql.Append(" FROM " + FormatTableName<TEntity>());
            if (whereFields != null && whereFields.Any())
            {
                sql.Append(" WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteFiled(p) + "=@" + p)));
            }
            return sql.ToString();
        }

        public virtual string FormatQueryPageSql<TEntity>(int page, int rows, List<string> searchFields, List<string> whereFields, string order) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
