using Ku.Core.Extensions.Dapper.SqlDialect;
using System;
using System.Data;

namespace Ku.Core.Extensions.Dapper
{
    public class DapperOptions
    {
        public DbType DbType { set; get; } = DbType.MySql;

        public Func<IDbConnection> DbConnection { get; set; }

        public ISqlDialect SqlDialect { set; get; }

        public int? Timeout { set; get; } = null;
    }

    public enum DbType
    {
        SqlServer,
        MySql
    }
}
