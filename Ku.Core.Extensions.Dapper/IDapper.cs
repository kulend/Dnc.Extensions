using System;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.Dapper
{
    public interface IDapper
    {
        TEntity QueryById<TEntity>(object id) where TEntity : class;

        Task<TEntity> QueryByIdAsync<TEntity>(object id) where TEntity : class;
    }
}
