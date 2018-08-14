using Dnc.Extensions.Dapper.SqlDialect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dnc.Extensions.Dapper
{
    public interface IDapper : IDisposable
    {
        IDbConnection Connection { get; }

        int? Timeout { get; }

        IDbTransaction DbTransaction { get; }

        ISqlDialect Dialect { set; get; }

        #region 事务

        ITransation BeginTrans();

        ITransation BeginTrans(IsolationLevel il);

        void Commit();

        void Rollback();

        #endregion

        #region 更新数据

        int Update<TEntity>(dynamic data, dynamic where = null) where TEntity : class;

        Task<int> UpdateAsync<TEntity>(dynamic data, dynamic where = null) where TEntity : class;

        #endregion

        #region 删除&逻辑删除

        int Delete<TEntity>(dynamic where) where TEntity : class;

        Task<int> DeleteAsync<TEntity>(dynamic where) where TEntity : class;

        #endregion

        #region 逻辑恢复

        int Restore<TEntity>(dynamic where) where TEntity : class;

        Task<int> RestoreAsync<TEntity>(dynamic where) where TEntity : class;

        #endregion

        void Log(string method, string message);
    }
}
