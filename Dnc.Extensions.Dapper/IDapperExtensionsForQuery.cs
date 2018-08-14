using Dapper;
using Dnc.Extensions.Dapper.Builders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dnc.Extensions.Dapper
{
    public static class IDapperExtensionsForQuery
    {
        #region QueryOne & QueryOneAsync

        public static TEntity QueryOne<TEntity>(this IDapper dapper, QueryBuilder builder) where TEntity : class
        {
            var result = builder.Build();

            dapper.Log("QueryOne", result.sql);
            return dapper.Connection.QueryFirstOrDefault<TEntity>(result.sql, result.param, dapper.DbTransaction, dapper.Timeout);
        }

        public static async Task<TEntity> QueryOneAsync<TEntity>(this IDapper dapper, QueryBuilder builder) where TEntity : class
        {
            var result = builder.Build();

            dapper.Log("QueryOneAsync", result.sql);
            return await dapper.Connection.QueryFirstOrDefaultAsync<TEntity>(result.sql, result.param, dapper.DbTransaction, dapper.Timeout);
        }

        public static TEntity QueryOne<TEntity>(this IDapper dapper, dynamic where, dynamic order = null) where TEntity : class
        {
            var builder = new SimpleQueryBuilder<TEntity>(where as object, order as object);

            return dapper.QueryOne<TEntity>(builder);
        }

        public static async Task<TEntity> QueryOneAsync<TEntity>(this IDapper dapper, dynamic where, dynamic order = null) where TEntity : class
        {
            var builder = new SimpleQueryBuilder<TEntity>(where as object, order as object);
            return await dapper.QueryOneAsync<TEntity>(builder);
        }

        #endregion

        #region QueryList & QueryListAsync

        public static IEnumerable<TEntity> QueryList<TEntity>(this IDapper dapper, QueryBuilder builder) where TEntity : class
        {
            var result = builder.Build();

            dapper.Log("QueryList", result.sql);
            return dapper.Connection.Query<TEntity>(result.sql, result.param, dapper.DbTransaction, true, dapper.Timeout);
        }

        public static async Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(this IDapper dapper, QueryBuilder builder) where TEntity : class
        {
            var result = builder.Build();

            dapper.Log("QueryListAsync", result.sql);
            return await dapper.Connection.QueryAsync<TEntity>(result.sql, result.param, dapper.DbTransaction, dapper.Timeout);
        }

        public static IEnumerable<TEntity> QueryList<TEntity>(this IDapper dapper, dynamic where, dynamic order = null) where TEntity : class
        {
            var builder = new SimpleQueryBuilder<TEntity>(where as object, order as object);

            return dapper.QueryList<TEntity>(builder);
        }

        public static async Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(this IDapper dapper, dynamic where, dynamic order = null) where TEntity : class
        {
            var builder = new SimpleQueryBuilder<TEntity>(where as object, order as object);

            return await dapper.QueryListAsync<TEntity>(builder);
        }

        #endregion

        #region QueryCount & QueryCountAsync

        /// <summary>
        /// 查询件数
        /// </summary>
        /// <returns>数据件数</returns>
        public static int QueryCount(this IDapper dapper, QueryBuilder builder)
        {
            var result = builder.Build();

            dapper.Log("QueryCount", result.countSql);
            return dapper.Connection.ExecuteScalar<int?>(result.countSql, result.param, dapper.DbTransaction, dapper.Timeout).GetValueOrDefault();
        }

        /// <summary>
        /// 查询件数
        /// </summary>
        /// <returns>数据件数</returns>
        public static int QueryCount<TEntity>(this IDapper dapper, dynamic where) where TEntity : class
        {
            var builder = new SimpleQueryBuilder<TEntity>(where as object, null);

            return dapper.QueryCount(builder);
        }

        /// <summary>
        /// 查询件数
        /// </summary>
        /// <returns>数据件数</returns>
        public static async Task<int> QueryCountAsync(this IDapper dapper, QueryBuilder builder)
        {
            var result = builder.Build();

            dapper.Log("QueryCountAsync", result.countSql);
            return (await dapper.Connection.ExecuteScalarAsync<int?>(result.countSql, result.param, dapper.DbTransaction, dapper.Timeout)).GetValueOrDefault();
        }

        /// <summary>
        /// 查询件数
        /// </summary>
        /// <returns>数据件数</returns>
        public static async Task<int> QueryCountAsync<TEntity>(this IDapper dapper, dynamic where) where TEntity : class
        {
            var builder = new SimpleQueryBuilder<TEntity>(where as object, null);

            return await dapper.QueryCountAsync(builder);
        }

        #endregion

        #region QueryPage & QueryPageAsync

        public static (int count, IEnumerable<TEntity> items) QueryPage<TEntity>(this IDapper dapper, QueryBuilder builder)
        {
            var result = builder.Build();

            dapper.Log("ExecuteScalar", result.countSql);
            var count = dapper.Connection.ExecuteScalar<int?>(result.countSql, result.param, dapper.DbTransaction, dapper.Timeout).GetValueOrDefault();

            var limit = builder.GetLimit();
            if (count == 0 || count <= ((limit.page - 1) * limit.rows)) return (count, new TEntity[] { });

            dapper.Log("QueryPage", result.pageSql);
            var items = dapper.Connection.Query<TEntity>(result.pageSql, result.param, dapper.DbTransaction, true, dapper.Timeout);
            return (count, items);
        }

        public static async Task<(int count, IEnumerable<TEntity> items)> QueryPageAsync<TEntity>(this IDapper dapper, QueryBuilder builder)
        {
            var result = builder.Build();

            dapper.Log("ExecuteScalarAsync", result.countSql);
            var count = (await dapper.Connection.ExecuteScalarAsync<int?>(result.countSql, result.param, dapper.DbTransaction, dapper.Timeout)).GetValueOrDefault();
            var limit = builder.GetLimit();
            if (count == 0 || count <= ((limit.page - 1) * limit.rows)) return (count, new TEntity[] { });

            dapper.Log("QueryPageAsync", result.pageSql);
            var items = await dapper.Connection.QueryAsync<TEntity>(result.pageSql, result.param, dapper.DbTransaction, dapper.Timeout);
            return (count, items);
        }

        public static async Task<(int count, IEnumerable<TReturn> items)> QueryPageAsync<TFirst, TSecond, TReturn>(this IDapper dapper, QueryBuilder builder, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        {
            var result = builder.Build();

            dapper.Log("ExecuteScalarAsync", result.countSql);
            var count = (await dapper.Connection.ExecuteScalarAsync<int?>(result.countSql, result.param, dapper.DbTransaction, dapper.Timeout)).GetValueOrDefault();
            var limit = builder.GetLimit();
            if (count == 0 || count <= ((limit.page - 1) * limit.rows)) return (count, new TReturn[] { });

            dapper.Log("QueryPageAsync", result.pageSql);
            var items = await dapper.Connection.QueryAsync(result.pageSql, map, result.param, dapper.DbTransaction, true, splitOn, dapper.Timeout);
            return (count, items);
        }

        public static (int count, IEnumerable<TReturn> items) QueryPage<TFirst, TSecond, TReturn>(this IDapper dapper, QueryBuilder builder, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        {
            var result = builder.Build();

            dapper.Log("ExecuteScalar", result.countSql);
            var count = dapper.Connection.ExecuteScalar<int?>(result.countSql, result.param, dapper.DbTransaction, dapper.Timeout).GetValueOrDefault();
            var limit = builder.GetLimit();
            if (count == 0 || count <= ((limit.page - 1) * limit.rows)) return (count, new TReturn[] { });

            dapper.Log("QueryPage", result.pageSql);
            var items = dapper.Connection.Query(result.pageSql, map, result.param, dapper.DbTransaction, true, splitOn, dapper.Timeout);
            return (count, items);
        }

        #endregion

    }
}
