using Dnc.Extensions.Dapper.SqlDialect;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dnc.Extensions.Dapper.QueryBuilder
{
    public abstract class BaseBuilder
    {
        protected Dictionary<string, object> param = new Dictionary<string, object>();

        protected ISqlDialect _dialect;

        public BaseBuilder()
        {
            var options = DapperFactory.services.GetService<IOptions<DapperOptions>>().Value;
            _dialect = options.SqlDialect;
        }

        public abstract (string sql, Dictionary<string, object> param) Build();

        protected string FormatFiled(string tableAlias, string field)
        {
            return string.IsNullOrEmpty(tableAlias) ? _dialect.QuoteField(field) : $"{tableAlias}.{_dialect.QuoteField(field)}";
        }

        protected string GetParameterName(string name)
        {
            var num = new Random().Next(0, 999);
            return "_p" + name + num;
        }
    }
}
