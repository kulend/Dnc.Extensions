using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.Dapper.SqlDialect
{
    public interface ISqlDialect
    {
        char QuoteStart { get; }
        char QuoteEnd { get; }
        char ParameterPrefix { get; }

        string QuoteFiled(string filed);

        string FormatTableName(string tableName, string tableSchema = null);
        string FormatTableName<TEntity>();

        string FormatInsertSql<TEntity>(List<string> fields) where TEntity : class;

        string FormatUpdateSql<TEntity>(List<string> updateFields, List<string> whereFields, string wherePrefix) where TEntity : class;

        string FormatUpdateSql(string tableName, string tableSchema, List<string> updateFields, List<string> whereFields, string wherePrefix);

        string FormatLogicalDeleteSql<TEntity>(string field, List<string> whereFields) where TEntity : class;

        string FormatDeleteSql<TEntity>(List<string> whereFields) where TEntity : class;
    }
}
