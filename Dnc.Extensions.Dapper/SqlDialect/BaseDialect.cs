using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Dnc.Extensions.Dapper.SqlDialect
{
    public class BaseDialect : ISqlDialect
    {
        public virtual string Concat
        {
            get { return "CONCAT"; }
        }

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

        public string QuoteField(string filed)
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
            return FormatUpdateSql(tableName, updateFields, whereFields, wherePrefix);
        }

        public virtual string FormatUpdateSql(string tableName, string tableSchema, List<string> updateFields, List<string> whereFields, string wherePrefix)
        {
            tableName = FormatTableName(tableName, tableSchema);
            return FormatUpdateSql(tableName, updateFields, whereFields, wherePrefix);
        }

        public virtual string FormatUpdateSql(string formatTableName, List<string> updateFields, List<string> whereFields, string wherePrefix)
        {
            var columns = string.Join(",", updateFields.Select(p => QuoteStart + p + QuoteEnd + "=@" + p));
            var where = "";
            if (whereFields.Any())
            {
                where = " WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteStart + p + QuoteEnd + "=@" + wherePrefix + p));
            }
            return $"UPDATE {formatTableName} SET {columns}{where}";
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
                where = "WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteField(p) + "=@" + p));
            }
            else if (!string.IsNullOrEmpty(whereSql))
            {
                where = "WHERE " + whereSql;
            }
            else
            {
                where = "WHERE 1<>1 ";
            }

            return $"UPDATE {FormatTableName<TEntity>()} SET {QuoteField(field)}=@{field} {where}";
        }

        public virtual string FormatDeleteSql<TEntity>(List<string> whereFields, string whereSql) where TEntity : class
        {
            var where = "WHERE 1<>1 ";
            if (whereFields != null && whereFields.Any())
            {
                where = "WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteField(p) + "=@" + p));
            } else if (!string.IsNullOrEmpty(whereSql))
            {
                where = "WHERE " + whereSql;
            }

            return $"DELETE FROM {FormatTableName<TEntity>()} {where}";
        }

        public virtual string FormatCountSql<TEntity>(List<string> whereFields, string whereSql)
        {
            var sql = new StringBuilder("SELECT COUNT(1)");
            sql.Append(" FROM " + FormatTableName<TEntity>());
            if (whereFields != null && whereFields.Any())
            {
                sql.Append(" WHERE " + string.Join(" AND ", whereFields.Select(p => QuoteField(p) + "=@" + p)));
            } else if (!string.IsNullOrEmpty(whereSql))
            {
                sql.Append(" WHERE " + whereSql);
            }
            return sql.ToString();
        }

        public virtual string FormatQueryPageSql<TEntity>(int page, int rows, string field, string where, string order) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual string FormatWhereSql(List<string> whereFields, string whereSql)
        {
            if ((whereFields == null || !whereFields.Any()) && string.IsNullOrEmpty(whereSql))
            {
                return "";
            }

            var sql = new StringBuilder(" WHERE ");
            if (whereFields != null && whereFields.Any())
            {
                sql.Append(string.Join(" AND ", whereFields.Select(p => QuoteField(p) + "=@" + p)));
                if (!string.IsNullOrEmpty(whereSql))
                {
                    sql.Append(" AND ");
                }
            }

            if (!string.IsNullOrEmpty(whereSql))
            {
                sql.Append(whereSql);
            }
            return sql.ToString();
        }

        public virtual string FormatOrderSql(Dictionary<string, string> fields, string orderSql)
        {
            if ((fields == null || !fields.Any()) && string.IsNullOrEmpty(orderSql))
            {
                return "";
            }

            var sql = new StringBuilder(" ORDER BY ");
            if (fields != null && fields.Any())
            {
                sql.Append(string.Join(",", fields.Select(p => QuoteField(p.Key) + ("desc".Equals(p.Value, StringComparison.OrdinalIgnoreCase) ? " DESC" : " ASC"))));
                if (!string.IsNullOrEmpty(orderSql))
                {
                    sql.Append(",");
                }
            }

            if (!string.IsNullOrEmpty(orderSql))
            {
                sql.Append(orderSql);
            }
            return sql.ToString();
        }

        protected virtual string ParseOrderSql(string order)
        {
            if (string.IsNullOrEmpty(order)) return "";

            var sql = "";
            var orderFileds = order.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());

            foreach (var item in orderFileds)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    sql += ",";
                }

                var spItem = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                bool desc = (spItem.Length > 1 && spItem[1].Equals("desc", StringComparison.OrdinalIgnoreCase));

                sql += QuoteField(spItem[0]) + (desc ? " DESC" : " ASC");
            }

            return " ORDER BY " + sql;
        }

        public virtual string FormatQuerySql(string fieldSql, string tableJoin, string where, string order, bool isOne)
        {
            throw new NotImplementedException();
        }

        public virtual string FormatQueryPageSql(int page, int rows, string sql)
        {
            throw new NotImplementedException();
        }
    }
}
