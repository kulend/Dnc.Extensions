using System;
using System.Collections.Generic;
using System.Text;

namespace Dnc.Extensions.Dapper.QueryBuilder
{
    public class JoinBuilder : DapperQueryBuilder
    {
        public JoinBuilder(StringBuilder sql): base ()
        {
            this.sql = sql;
        }

    }
}
