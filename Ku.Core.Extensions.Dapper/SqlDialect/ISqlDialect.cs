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

        string GetTableNameWithSchema<TEntity>() where TEntity : class;
    }
}
