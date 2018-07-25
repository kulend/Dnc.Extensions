using Dapper;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Ku.Core.Extensions.Dapper
{
    public static class IDapperExtensions
    {
        public static IEnumerable<T> Query<T>(this IDapper dapper, string sql, object param = null, bool buffered = true)
        {
            return dapper.Connection.Query<T>(sql, param, dapper.DbTransaction, buffered);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return dapper.Connection.Query(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return dapper.Connection.Query(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return dapper.Connection.Query(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return dapper.Connection.Query(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return dapper.Connection.Query(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return dapper.Connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        //[return: Dynamic(new[] { false, true })]
        //public static IEnumerable<dynamic> Query(this IDapper dapper, string sql, object param = null, bool buffered = true);
        //{
        //    return dapper.Connection.Query(sql, map, param, dapper.DbTransaction, buffered);
        //}

        //public static IEnumerable<object> Query(this IDapper dapper, Type type, string sql, object param = null, bool buffered = true)
        //{
        //    return dapper.Connection.Query(sql, param, dapper.DbTransaction, buffered);
        //}

        //public static IEnumerable<TReturn> Query<TReturn>(this IDapper dapper, string sql, Type[] types, Func<object[], TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        //{
        //    return dapper.Connection.Query<TReturn>(sql, types, map, param, dapper.DbTransaction, buffered, splitOn);
        //}

        //public static IEnumerable<T> Query<T>(this IDapper dapper, CommandDefinition command)
        //{
        //    return dapper.Connection.Query<T>(command);
        //}

        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDapper dapper, string sql, object param = null)
        {
            return await dapper.Connection.QueryAsync<T>(sql, param, dapper.DbTransaction);
        }

        public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return await dapper.Connection.QueryAsync<TFirst, TSecond, TReturn>(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return await dapper.Connection.QueryAsync<TFirst, TSecond, TThird, TReturn>(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return await dapper.Connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return await dapper.Connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return await dapper.Connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDapper dapper, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            return await dapper.Connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(sql, map, param, dapper.DbTransaction, buffered, splitOn);
        }

        //public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDapper dapper, CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id")
        //{
        //    return await dapper.Connection.QueryAsync<TFirst, TSecond, TThird, TReturn>(command, map, splitOn);
        //}


        //public static async Task<IEnumerable<object>> QueryAsync(this IDapper dapper, Type type, CommandDefinition command)
        //{
        //    return await dapper.Connection.QueryAsync(type, command);
        //}

        //[return: Dynamic(new[] { false, false, true })]
        //public static async Task<IEnumerable<dynamic>> QueryAsync(this IDapper dapper, CommandDefinition command);

        //public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDapper dapper, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn = "Id")
        //{
        //    return await dapper.Connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(command, map, splitOn);
        //}

        //public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDapper dapper, CommandDefinition command, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        //{

        //}

        //public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDapper dapper, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn = "Id")
        //{

        //}

        //public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDapper dapper, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn = "Id")
        //{

        //}

        //public static async Task<IEnumerable<object>> QueryAsync(this IDapper dapper, Type type, string sql, object param = null, IDbTransaction transaction = null)
        //{

        //}

        //[return: Dynamic(new[] { false, false, true })]
        //public static async Task<IEnumerable<dynamic>> QueryAsync(this IDapper dapper, string sql, object param = null, IDbTransaction transaction = null);

        //public static async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDapper dapper, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn = "Id")
        //{

        //}

        //public static async Task<IEnumerable<TReturn>> QueryAsync<TReturn>(this IDapper dapper, string sql, Type[] types, Func<object[], TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        //{

        //}

        //public static async Task<IEnumerable<T>> QueryAsync<T>(this IDapper dapper, CommandDefinition command)
        //{

        //}

        public static GridReader QueryMultiple(this IDapper dapper, string sql, object param = null)
        {
            return dapper.Connection.QueryMultiple(sql, param, dapper.DbTransaction, dapper.Timeout, null);
        }

        public static async Task<GridReader> QueryMultipleAsync(this IDapper dapper, string sql, object param = null)
        {
            return await dapper.Connection.QueryMultipleAsync(sql, param, dapper.DbTransaction, dapper.Timeout, null);
        }

        //public static (int count, IEnumerable<T> items) QueryPage<T>(this IDapper dapper, int page, int size, string sql, string sqlCount, object param, bool buffered = true)
        //{
        //    //取得件数
        //    var count = dapper.Connection.ExecuteScalar<int?>(sqlCount, param, dapper.DbTransaction, dapper.Timeout).GetValueOrDefault();
        //    if (count == 0)
        //    {
        //        return (0, new T[] { });
        //    }

        //    if (count <= ((page - 1) * size))
        //    {
        //        return (count, new T[] { });
        //    }

        //    var items = dapper.Connection.Query<T>(dapper.Dialect.FormatQueryPageSql(page, size, sql), param, dapper.DbTransaction, buffered);
        //    return (count, items);
        //}

        //public static async Task<(int count, IEnumerable<T> items)> QueryPageAsync<T>(this IDapper dapper, int page, int size, string sql, string sqlCount, object param)
        //{
        //    //取得件数
        //    var count = (await dapper.Connection.ExecuteScalarAsync<int?>(sqlCount, param, dapper.DbTransaction, dapper.Timeout)).GetValueOrDefault();
        //    if (count == 0)
        //    {
        //        return (0, new T[] { });
        //    }

        //    if (count <= ((page - 1) * size))
        //    {
        //        return (count, new T[] { });
        //    }

        //    var items = await dapper.Connection.QueryAsync<T>(dapper.Dialect.FormatQueryPageSql(page, size, sql), param, dapper.DbTransaction, dapper.Timeout);
        //    return (count, items);
        //}

        //public static (int count, IEnumerable<TReturn> items) QueryPage<TFirst, TSecond, TReturn>(this IDapper dapper, int page, int size, string sql, string sqlCount, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        //{
        //    //取得件数
        //    dapper.Log("ExecuteScalar", sqlCount);
        //    var count = dapper.Connection.ExecuteScalar<int?>(sqlCount, param, dapper.DbTransaction, dapper.Timeout).GetValueOrDefault();
        //    if (count == 0)
        //    {
        //        return (0, new TReturn[] { });
        //    }

        //    if (count <= ((page - 1) * size))
        //    {
        //        return (count, new TReturn[] { });
        //    }
        //    var s = dapper.Dialect.FormatQueryPageSql(page, size, sql);
        //    dapper.Log("Query", s);
        //    var items = dapper.Connection.Query(s, map, param, dapper.DbTransaction, buffered, splitOn);
        //    return (count, items);
        //}

        //public static async Task<(int count, IEnumerable<TReturn> items)> QueryPageAsync<TFirst, TSecond, TReturn>(this IDapper dapper, int page, int size, string sql, string sqlCount, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        //{
        //    //取得件数
        //    dapper.Log("ExecuteScalarAsync", sqlCount);
        //    var count = (await dapper.Connection.ExecuteScalarAsync<int?>(sqlCount, param, dapper.DbTransaction, dapper.Timeout)).GetValueOrDefault();
        //    if (count == 0)
        //    {
        //        return (0, new TReturn[] { });
        //    }

        //    if (count <= ((page - 1) * size))
        //    {
        //        return (count, new TReturn[] { });
        //    }
        //    var s = dapper.Dialect.FormatQueryPageSql(page, size, sql);
        //    dapper.Log("QueryAsync", s);
        //    var items = await dapper.Connection.QueryAsync(s, map, param, dapper.DbTransaction, buffered, splitOn);
        //    return (count, items);
        //}

        public static async Task<int> ExecuteAsync(this IDapper dapper, string sql, object param = null)
        {
            return await dapper.Connection.ExecuteAsync(sql, param, dapper.DbTransaction, dapper.Timeout, null);
        }
    }
}
