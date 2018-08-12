using Dnc.Extensions.Dapper.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dnc.Extensions.Dapper.QueryBuilder
{
    public class LogicConditionBuilder : BaseBuilder
    {
        protected StringBuilder sb = new StringBuilder();

        public LogicConditionBuilder Equal<T1>(Expression<Func<T1, object>> field, object value)
        {
            var t1 = typeof(T1).FullName;
            var n1 = ExpressionHelper.GetPropertyName(field);

            var p = GetParameterName(n1);
            param.Add(p, value);

            sb.Append($"{FormatFiled(t1, n1)}=@{p}");

            return this;
        }

        public LogicConditionBuilder Equal<T1, T2>(Expression<Func<T1, object>> field1, Expression<Func<T1, object>> field2)
        {
            var t1 = typeof(T1).FullName;
            var n1 = ExpressionHelper.GetPropertyName(field1);

            var t2 = typeof(T2).FullName;
            var n2 = ExpressionHelper.GetPropertyName(field2);

            sb.Append($"{FormatFiled(t1, n1)}={FormatFiled(t2, n2)}");

            return this;
        }

        public override (string sql, Dictionary<string, object> param) Build()
        {
            return (sb.ToString(), param);
        }
    }
}
