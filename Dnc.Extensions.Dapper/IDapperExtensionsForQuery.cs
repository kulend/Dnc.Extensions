using Dapper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dnc.Extensions.Dapper
{
    public static class IDapperExtensionsForQuery
    {
        private static (string sql, DynamicParameters parameters) _Query(IDapper dapper, string field, object tableJoin, object where, object order, bool isOne)
        {
            DynamicParameters parameters;

            //处理WHERE语句
            string whereSql;
            if (where is DapperSql dSql)
            {
                whereSql = dapper.Dialect.FormatWhereSql(null, dSql.Sql);
                parameters = new DynamicParameters(dSql.Parameters as object);
            }
            else if (where is string s)
            {
                whereSql = dapper.Dialect.FormatWhereSql(null, s);
                parameters = new DynamicParameters();
            }
            else
            {
                parameters = new DynamicParameters(where);
                var fields = Dapper.GetDynamicFields(where).Select(x => x.Name).ToList();
                whereSql = dapper.Dialect.FormatWhereSql(fields, null);
            }

            //处理ORDER语句
            string orderSql = null;
            if (order is DapperSql dSqlOrder)
            {
                orderSql = dapper.Dialect.FormatOrderSql(null, dSqlOrder.Sql);
            }
            else if (order is string s)
            {
                orderSql = dapper.Dialect.FormatOrderSql(null, s);
            }
            else if (order != null)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                Dapper.GetDynamicFields(order).ForEach(item => dict.Add(item.Name, (string)item.Value));
                orderSql = dapper.Dialect.FormatOrderSql(dict, null);
            }

            //处理tableJoin
            string tableJoinSql = null;
            if (tableJoin is string str1)
            {
                tableJoinSql = str1;
            }
            else if (tableJoin is DapperSql ds)
            {
                tableJoinSql = ds.Sql;
                var dynamicFields = Dapper.GetDynamicFields(ds.Parameters as object);
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                dynamicFields.ForEach(p => expandoObject.Add(p.Name, p.Value));
                parameters.AddDynamicParams(expandoObject);
            }

            var sql = dapper.Dialect.FormatQuerySql(field, tableJoinSql, whereSql, orderSql, isOne);
            return (sql, parameters);
        }

        public static IEnumerable<TEntity> QueryList<TEntity>(this IDapper dapper, dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Query(dapper, "*", dapper.Dialect.FormatTableName<TEntity>(), where as object, null as object, false);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            dapper.Log("QueryList", sql);
            return dapper.Connection.Query<TEntity>(sql, parameters, dapper.DbTransaction, true, dapper.Timeout);
        }

        #region QueryList

        public static IEnumerable<TEntity> QueryList<TEntity>(this IDapper dapper, dynamic where, dynamic order = null) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Query(dapper, "*", dapper.Dialect.FormatTableName<TEntity>(), where as object, order as object, false);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            dapper.Log("QueryList", sql);
            return dapper.Connection.Query<TEntity>(sql, parameters, dapper.DbTransaction, true, dapper.Timeout);
        }

        public static IEnumerable<TEntity> QueryList<TEntity>(this IDapper dapper, string field, dynamic tableJoin, dynamic where, dynamic order = null)
        {
            (string sql, DynamicParameters parameters) = _Query(dapper, field, tableJoin as object, where as object, order as object, false);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            dapper.Log("QueryList", sql);
            return dapper.Connection.Query<TEntity>(sql, parameters, dapper.DbTransaction, true, dapper.Timeout);
        }

        public static IEnumerable<TReturn> QueryList<TFirst, TSecond, TReturn>(this IDapper dapper, string field, dynamic tableJoin, dynamic where, dynamic order, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        {
            (string sql, DynamicParameters parameters) = _Query(dapper, field, tableJoin as object, where as object, order as object, false);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            dapper.Log("QueryList", sql);
            return dapper.Connection.Query(sql, map, parameters, dapper.DbTransaction, true, splitOn);
        }

        #endregion

        #region QueryListAsync

        public static async Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(this IDapper dapper, dynamic where, dynamic order = null) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Query(dapper, "*", dapper.Dialect.FormatTableName<TEntity>(), where as object, order as object, false);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            dapper.Log("QueryListAsync", sql);
            return await dapper.Connection.QueryAsync<TEntity>(sql, parameters, dapper.DbTransaction, dapper.Timeout);
        }

        public static async Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(this IDapper dapper, string field, dynamic tableJoin, dynamic where, dynamic order = null)
        {
            (string sql, DynamicParameters parameters) = _Query(dapper, field, tableJoin as object, where as object, order as object, false);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            dapper.Log("QueryListAsync", sql);
            return await dapper.Connection.QueryAsync<TEntity>(sql, parameters, dapper.DbTransaction, dapper.Timeout);
        }

        public static async Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TReturn>(this IDapper dapper, string field, dynamic tableJoin, dynamic where, dynamic order, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        {
            (string sql, DynamicParameters parameters) = _Query(dapper, field, tableJoin as object, where as object, order as object, false);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            dapper.Log("QueryListAsync", sql);
            return await dapper.Connection.QueryAsync(sql, map, parameters, dapper.DbTransaction, true, splitOn);
        }

        #endregion

    }
}
