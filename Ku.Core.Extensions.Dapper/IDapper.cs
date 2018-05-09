using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.Dapper
{
    public interface IDapper : IDisposable
    {
        #region 事务

        ITransation BeginTrans();

        ITransation BeginTrans(IsolationLevel il);

        void Commit();

        void Rollback();

        #endregion

        #region 查询

        TEntity QueryOne<TEntity>(dynamic where, string order = null) where TEntity : class;

        Task<TEntity> QueryOneAsync<TEntity>(dynamic where, string order = null) where TEntity : class;

        IEnumerable<TEntity> QueryList<TEntity>(dynamic where, string order = null) where TEntity : class;

        Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(dynamic where, string order = null) where TEntity : class;

        (int count, IEnumerable<TEntity> items) QueryPage<TEntity>(int page, int size, dynamic where, string order = null) where TEntity : class;

        Task<(int count, IEnumerable<TEntity> items)> QueryPageAsync<TEntity>(int page, int size, dynamic where, string order = null) where TEntity : class;

        #endregion

        #region 件数

        int QueryCount<TEntity>(dynamic where) where TEntity : class;

        Task<int> QueryCountAsync<TEntity>(dynamic where) where TEntity : class;

        #endregion

        #region 插入数据

        bool Insert<TEntity>(TEntity entity) where TEntity : class;

        Task<bool> InsertAsync<TEntity>(TEntity entity) where TEntity : class;

        #endregion

        #region 更新数据

        int Update<TEntity>(dynamic data, dynamic where = null) where TEntity : class;

        Task<int> UpdateAsync<TEntity>(dynamic data, dynamic where = null) where TEntity : class;

        //int Update<TEntity>(TEntity entity) where TEntity : class;

        //int Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] updateFileds) where TEntity : class;

        //int Update<TEntity>(TEntity entity, params string[] updateFileds) where TEntity : class;

        int UpdateExt(string table, string tableSchema, dynamic data, dynamic where);

        //Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

        //Task<int> UpdateAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] updateFileds) where TEntity : class;

        //Task<int> UpdateAsync<TEntity>(TEntity entity, params string[] updateFileds) where TEntity : class;

        Task<int> UpdateExtAsync(string table, string tableSchema, dynamic data, dynamic where);

        #endregion

        #region 删除&逻辑删除

        int Delete<TEntity>(dynamic where) where TEntity : class;

        Task<int> DeleteAsync<TEntity>(dynamic where) where TEntity : class;

        #endregion

        #region 逻辑恢复

        int Restore<TEntity>(dynamic where) where TEntity : class;

        Task<int> RestoreAsync<TEntity>(dynamic where) where TEntity : class;

        #endregion
    }
}
