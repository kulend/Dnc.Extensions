using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dnc.Extensions.Dapper
{
    public static class Extensions
    {
        public static void TryConcat<K, V>(this IDictionary<K, V> self, params IDictionary<K, V>[] others)
        {
            if (others == null || others.Length == 0)
            {
                return;
            }
            if (self == null)
            {
                self = new Dictionary<K, V>();
            }
            IEnumerable<KeyValuePair<K, V>> list = self;
            foreach (var other in others.Where(x => x != null && x.Count > 0))
            {
                foreach (var item in other)
                {
                    self.Add(item);
                }
            }
        }

        public static string GetPropertyName<TSource, TField>(this Expression<Func<TSource, TField>> field)
        {
            if (Equals(field, null))
                throw new ArgumentNullException(nameof(field), "field can't be null");

            MemberExpression expr;

            switch (field.Body)
            {
                case MemberExpression body:
                    expr = body;
                    break;
                case UnaryExpression expression:
                    expr = (MemberExpression)expression.Operand;
                    break;
                default:
                    throw new ArgumentException("Expression field isn't supported", nameof(field));
            }

            return expr.Member.Name;
        }
    }
}
