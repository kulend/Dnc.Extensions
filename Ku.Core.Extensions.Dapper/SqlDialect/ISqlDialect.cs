using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.Dapper.SqlDialect
{
    public interface ISqlDialect
    {
        string Concat { get; }
        char QuoteStart { get; }
        char QuoteEnd { get; }
        char ParameterPrefix { get; }

        string QuoteFiled(string filed);

        string FormatTableName(string tableName, string tableSchema = null);
        string FormatTableName<TEntity>();

        string FormatInsertSql<TEntity>(List<string> fields) where TEntity : class;

        string FormatUpdateSql<TEntity>(List<string> updateFields, List<string> whereFields, string wherePrefix) where TEntity : class;

        string FormatUpdateSql(string tableName, string tableSchema, List<string> updateFields, List<string> whereFields, string wherePrefix);

        string FormatUpdateSql(string formatTableName, List<string> updateFields, List<string> whereFields, string wherePrefix);

        string FormatLogicalDeleteRestoreSql<TEntity>(string field, List<string> whereFields = null, string whereSql = null) where TEntity : class;

        string FormatDeleteSql<TEntity>(List<string> whereFields, string whereSql) where TEntity : class;

        string FormatCountSql<TEntity>(List<string> whereFields, string whereSql);

        string FormatQueryPageSql<TEntity>(int page, int rows, string field, string where, string order) where TEntity : class;

        string FormatWhereSql(List<string> whereFields, string whereSql);

        string FormatOrderSql(Dictionary<string, string> fields, string orderSql);

        string FormatQuerySql(string fieldSql, string tableJoin, string where, string order, bool isOne);

        string FormatQueryPageSql(int page, int rows, string sql);
    }
}
