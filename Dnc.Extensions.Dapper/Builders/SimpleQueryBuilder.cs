using System;
using System.Collections.Generic;
using System.Text;

namespace Dnc.Extensions.Dapper.Builders
{
    public class SimpleQueryBuilder<TEntity> : QueryBuilder where TEntity : class
    {
        public SimpleQueryBuilder(dynamic where, dynamic sort) : base()
        {
            Select<TEntity>().From<TEntity>();
            if (where as object != null)
            {
                Where(ConditionBuilder.FormDynamic<TEntity>(where as object));
            }
            if (sort as object != null)
            {
                Sort(sort as object);
            }
        }
    }
}
