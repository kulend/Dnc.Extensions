using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dnc.Extensions.Dapper
{
    public static class IDapperExtensionsForInsert
    {
        #region 插入数据

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns>操作数据条数</returns>
        public static int Insert<TEntity>(this IDapper dapper, params TEntity[] entitys) where TEntity : class
        {
            if (entitys == null || entitys.Length == 0)
            {
                throw new DapperException("插入的数据不能为空！");
            }
            var fields = Dapper.GetDynamicFields(entitys.First()).Select(x => x.Name).ToList();
            var sql = dapper.Dialect.FormatInsertSql<TEntity>(fields);
            dapper.Log("Insert", $"[{entitys.Length}]" + sql);
            return dapper.Connection.Execute(sql, entitys, dapper.DbTransaction, dapper.Timeout);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns>操作数据条数</returns>
        public static async Task<int> InsertAsync<TEntity>(this IDapper dapper, params TEntity[] entitys) where TEntity : class
        {
            if (entitys == null || entitys.Length == 0)
            {
                throw new DapperException("插入的数据不能为空！");
            }
            var fields = Dapper.GetDynamicFields(entitys.First()).Select(x => x.Name).ToList();
            var sql = dapper.Dialect.FormatInsertSql<TEntity>(fields);
            dapper.Log("InsertAsync", $"[{entitys.Length}]" + sql);
            return await dapper.Connection.ExecuteAsync(sql, entitys, dapper.DbTransaction, dapper.Timeout);
        }

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <returns>操作数据条数</returns>
        public static int Insert<TEntity>(this IDapper dapper, IEnumerable<TEntity> entitys) where TEntity : class
        {
            if (entitys == null || !entitys.Any())
            {
                throw new DapperException("插入的数据不能为空！");
            }
            var fields = Dapper.GetDynamicFields(entitys.First()).Select(x => x.Name).ToList();
            var sql = dapper.Dialect.FormatInsertSql<TEntity>(fields);
            dapper.Log("Insert", $"[{entitys.Count()}]" + sql);
            return dapper.Connection.Execute(sql, entitys, dapper.DbTransaction, dapper.Timeout);
        }

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <returns>操作数据条数</returns>
        public static async Task<int> InsertAsync<TEntity>(this IDapper dapper, IEnumerable<TEntity> entitys) where TEntity : class
        {
            if (entitys == null || !entitys.Any())
            {
                return 0;
            }
            var fields = Dapper.GetDynamicFields(entitys.First()).Select(x => x.Name).ToList();
            var sql = dapper.Dialect.FormatInsertSql<TEntity>(fields);
            dapper.Log("InsertAsync", $"[{entitys.Count()}]" + sql);
            return await dapper.Connection.ExecuteAsync(sql, entitys, dapper.DbTransaction, dapper.Timeout);
        }

        #endregion
    }
}
