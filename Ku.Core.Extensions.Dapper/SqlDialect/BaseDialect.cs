using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

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

        public virtual string GetTableNameWithSchema<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            var attr = type.GetCustomAttribute<TableAttribute>();
            var tableName = attr != null ? attr.Name : type.Name;
            var tableSchema = attr != null ? attr.Schema : string.Empty;

            return !string.IsNullOrEmpty(tableSchema)
                ? QuoteStart + tableSchema + QuoteEnd + "." + QuoteStart + tableName + QuoteEnd
                : QuoteStart + tableName + QuoteEnd;
        }

        public virtual string GetPageQuerySql<TEntity>(int page, int rows, string orderBy) where TEntity : class
        {
            return "";
        }
    }
}
