using Dnc.Extensions.Dapper.SqlDialect;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dnc.Extensions.Dapper.Builders
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

        public string FormatFiled(string tableAlias, string field)
        {
            return string.IsNullOrEmpty(tableAlias) ? _dialect.QuoteField(field) : $"{tableAlias}.{_dialect.QuoteField(field)}";
        }

        public string GetParameterName(string name = null)
        {
            var num = new Random().Next(0, 999);
            return "_p" + name + num;
        }

        public static string FormatTableAliasKey<TEntity>()
        {
            var key = typeof(TEntity).FullName;
            key = key.Replace("+", ".");
            return "@@" + key + "@@";
        }
    }
}
