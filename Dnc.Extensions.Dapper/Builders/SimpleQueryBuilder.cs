using System;
using System.Collections.Generic;
using System.Text;

namespace Dnc.Extensions.Dapper.Builders
{
    public class SimpleQueryBuilder<TEntity> : QueryBuilder where TEntity : class
    {
        public SimpleQueryBuilder(dynamic where, dynamic sort) : base()
        {
           Select<TEntity>().From<TEntity>().Where(ConditionBuilder.FormDynamic<TEntity>(where as object)).Sort(sort as object);
        }
    }
}
