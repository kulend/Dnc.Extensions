using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.Dapper
{
    public class DapperSql
    {
        public string Sql { set; get; }

        public dynamic Parameters { set; get; }

        public DapperSql(string sql)
        {
            this.Sql = sql;
        }

        public DapperSql(string sql, dynamic parameters)
        {
            this.Sql = sql;
            this.Parameters = parameters;
        }
    }
}
