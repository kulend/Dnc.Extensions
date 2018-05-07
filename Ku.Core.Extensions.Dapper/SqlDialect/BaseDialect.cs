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

        public virtual string GetPageQuerySql<TEntity>(int page, int rows, string orderBy) where TEntity : class
        {
            return "";
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

        public string FormatLogicalDeleteSql<TEntity>(string field, List<string> whereFields) where TEntity : class
        {
            var where = "";
            if (whereFields != null && whereFields.Any())
            {
                where = "WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteFiled(p) + "=@" + p));
            }
            else
            {
                where = "WHERE 1<>1 ";
            }

            return $"UPDATE {FormatTableName<TEntity>()} SET {QuoteFiled(field)}=@{field} {where}";
        }

        public string FormatDeleteSql<TEntity>(List<string> whereFields) where TEntity : class
        {
            var where = "WHERE 1<>1 ";
            if (whereFields != null && whereFields.Any())
            {
                where = "WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteFiled(p) + "=@" + p));
            }

            return $"DELETE FROM {FormatTableName<TEntity>()} {where}";
        }
    }
}
