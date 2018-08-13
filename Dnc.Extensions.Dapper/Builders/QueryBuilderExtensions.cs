using Dapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dnc.Extensions.Dapper.Builders
{
    public static class QueryBuilderExtensions
    {
        public static QueryBuilder Where<TEntity>(this QueryBuilder builder, DapperSearch<TEntity> search)
        {
            builder.Where(ConditionBuilder.FromSearch<TEntity>(search));
            return builder;
        }

        public static QueryBuilder Where<TEntity>(this QueryBuilder builder, object where)
        {
            builder.Where(ConditionBuilder.FormDynamic<TEntity>(where));
            return builder;
        }
    }
}
