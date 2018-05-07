using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ku.Core.Extensions.Dapper.Sql
{
    public sealed class SQLCreater
    {
        public string TableName { set; get; }
        public string TableSchema { set; get; }

        private string fields;
        private StringBuilder where;
        private Dictionary<string, object> paramters;

        public SQLCreater(IOptions<DapperOptions> options)
        {
            where = new StringBuilder();
            paramters = new Dictionary<string, object>();
        }

        public void addExpression(string symbol, string expression, params KeyValuePair<string, object>[] pms)
        {
            where.Append(" " + symbol + " ");
            where.Append(expression + "");
            foreach (var pm in pms)
            {
                if (!paramters.ContainsKey(pm.Key))
                {
                    paramters.Add(pm.Key, pm.Value);
                }
            }
        }
    }
}
