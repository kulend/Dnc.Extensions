using Dapper;
using Ku.Core.Extensions.Dapper.Attributes;
using Ku.Core.Extensions.Dapper.SqlDialect;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.Dapper
{
    internal class Dapper : IDapper
    {
        private readonly ILogger<Dapper> _logger;

        private DapperOptions _options;
        private ITransation _transaction;
        public IDbTransaction DbTransaction { get { return _transaction?.Transaction; } }

        public int? Timeout { private set; get; } = null;

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public IDbConnection Connection { get; private set; }

        /// <summary>
        /// Sql语法工具
        /// </summary>
        public ISqlDialect Dialect { set; get; }

        public Dapper(IOptions<DapperOptions> options, ILogger<Dapper> logger)
        {
            this._logger = logger;
            this._options = options.Value;
            this.Connection = _options.DbConnection();
            this.Dialect = _options.SqlDialect;
            Timeout = _options.Timeout;
        }

        #region 事务

        public ITransation BeginTrans()
        {
            if (_transaction == null)
            {
                if (this.Connection.State != ConnectionState.Open)
                {
                    this.Connection.Open();
                }
                _transaction = new DapperTransation(this.Connection.BeginTransaction());
            }
            return _transaction;
        }

        public ITransation BeginTrans(IsolationLevel il)
        {
            if (_transaction == null)
            {
                if (this.Connection.State != ConnectionState.Open)
                {
                    this.Connection.Open();
                }
                _transaction = new DapperTransation(this.Connection.BeginTransaction(il));
            }
            return _transaction;
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        #endregion

        #region 查询

        #region QueryOne

        public TEntity QueryOne<TEntity>(dynamic where, dynamic order = null) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Query("*", Dialect.FormatTableName<TEntity>(), where as object, order as object, true);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]QueryOne:" + sql);
            return Connection.QueryFirstOrDefault<TEntity>(sql, parameters, DbTransaction, Timeout);
        }

        #endregion

        #region QueryOneAsync

        public async Task<TEntity> QueryOneAsync<TEntity>(dynamic where, dynamic order = null) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Query("*", Dialect.FormatTableName<TEntity>(), where as object, order as object, true);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]QueryOneAsync:" + sql);
            return await Connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, DbTransaction, Timeout);
        } 

        #endregion

        //#region QueryList

        //public IEnumerable<TEntity> QueryList<TEntity>(dynamic where, dynamic order = null) where TEntity : class
        //{
        //    (string sql, DynamicParameters parameters) = _Query("*", Dialect.FormatTableName<TEntity>(), where as object, order as object, false);
        //    if (string.IsNullOrEmpty(sql))
        //    {
        //        throw new DapperException("SQL异常！");
        //    }
        //    _logger.LogDebug("[Dapper]QueryList:" + sql);
        //    return Connection.Query<TEntity>(sql, parameters, DbTransaction, true, Timeout);
        //}

        //public IEnumerable<TEntity> QueryList<TEntity>(string field, dynamic tableJoin, dynamic where, dynamic order = null) where TEntity : class
        //{
        //    (string sql, DynamicParameters parameters) = _Query(field, tableJoin as object, where as object, order as object, false);
        //    if (string.IsNullOrEmpty(sql))
        //    {
        //        throw new DapperException("SQL异常！");
        //    }
        //    _logger.LogDebug("[Dapper]QueryList:" + sql);
        //    return Connection.Query<TEntity>(sql, parameters, DbTransaction, true, Timeout);
        //}

        //#endregion

        //#region QueryListAsync

        //public async Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(dynamic where, dynamic order = null) where TEntity : class
        //{
        //    (string sql, DynamicParameters parameters) = _Query("*", Dialect.FormatTableName<TEntity>(), where as object, order as object, false);
        //    if (string.IsNullOrEmpty(sql))
        //    {
        //        throw new DapperException("SQL异常！");
        //    }
        //    _logger.LogDebug("[Dapper]QueryAsync:" + sql);
        //    return await Connection.QueryAsync<TEntity>(sql, parameters, DbTransaction, Timeout);
        //}

        //public async Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(string field, dynamic tableJoin, dynamic where, dynamic order = null)
        //{
        //    (string sql, DynamicParameters parameters) = _Query(field, tableJoin as object, where as object, order as object, false);
        //    if (string.IsNullOrEmpty(sql))
        //    {
        //        throw new DapperException("SQL异常！");
        //    }
        //    _logger.LogDebug("[Dapper]QueryAsync:" + sql);
        //    return await Connection.QueryAsync<TEntity>(sql, parameters, DbTransaction, Timeout);
        //}

        //public async Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TReturn>(string field, dynamic tableJoin, dynamic where, dynamic order, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        //{
        //    (string sql, DynamicParameters parameters) = _Query(field, tableJoin as object, where as object, order as object, false);
        //    if (string.IsNullOrEmpty(sql))
        //    {
        //        throw new DapperException("SQL异常！");
        //    }
        //    _logger.LogDebug("[Dapper]QueryListAsync:" + sql);
        //    return await Connection.QueryAsync(sql, map, parameters, DbTransaction, true, splitOn);
        //}

        //#endregion

        private (string sql, DynamicParameters parameters) _Query(string field, object tableJoin, object where, object order, bool isOne)
        {
            DynamicParameters parameters;

            //处理WHERE语句
            string whereSql;
            if (where is DapperSql dSql)
            {
                whereSql = Dialect.FormatWhereSql(null, dSql.Sql);
                parameters = new DynamicParameters(dSql.Parameters as object);
            }
            else if (where is string s)
            {
                whereSql = Dialect.FormatWhereSql(null, s);
                parameters = new DynamicParameters();
            }
            else
            {
                parameters = new DynamicParameters(where);
                var fields = GetDynamicFields(where).Select(x => x.Name).ToList();
                whereSql = Dialect.FormatWhereSql(fields, null);
            }

            //处理ORDER语句
            string orderSql = null;
            if (order is DapperSql dSqlOrder)
            {
                orderSql = Dialect.FormatOrderSql(null, dSqlOrder.Sql);
            }
            else if (order is string s)
            {
                orderSql = Dialect.FormatOrderSql(null, s);
            }
            else if (order != null)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                GetDynamicFields(order).ForEach(item => dict.Add(item.Name, (string)item.Value));
                orderSql = Dialect.FormatOrderSql(dict, null);
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
                var dynamicFields = GetDynamicFields(ds.Parameters as object);
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                dynamicFields.ForEach(p => expandoObject.Add(p.Name, p.Value));
                parameters.AddDynamicParams(expandoObject);
            }

            var sql = Dialect.FormatQuerySql(field, tableJoinSql, whereSql, orderSql, isOne);
            return (sql, parameters);
        }

        #endregion

        #region 分页查询

        public (int count, IEnumerable<TEntity> items) QueryPage<TEntity>(int page, int size, dynamic where, dynamic order = null) where TEntity : class
        {
            (string sql, string sqlCount, DynamicParameters parameters) = _QueryPage(page, size, "*", Dialect.FormatTableName<TEntity>(), where as object, order as object);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]QueryPageAsync:" + sqlCount);
            var count = (Connection.ExecuteScalar<int?>(sqlCount, parameters, DbTransaction, Timeout)).GetValueOrDefault();
            if (count == 0 || count <= ((page - 1) * size)) return (count, new TEntity[] { });

            _logger.LogDebug("[Dapper]QueryPageAsync:" + sql);
            var items = Connection.Query<TEntity>(sql, parameters, DbTransaction, true, Timeout);
            return (count, items);
        }

        public (int count, IEnumerable<TReturn> items) QueryPage<TFirst, TSecond, TReturn>(int page, int size, string feild, string tableJoin, dynamic where, dynamic order, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        {
            (string sql, string sqlCount, DynamicParameters parameters) = _QueryPage(page, size, feild, tableJoin, where as object, order as object);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]QueryPage:" + sqlCount);
            var count = (Connection.ExecuteScalar<int?>(sqlCount, parameters, DbTransaction, Timeout)).GetValueOrDefault();
            if (count == 0 || count <= ((page - 1) * size)) return (count, new TReturn[] { });

            _logger.LogDebug("[Dapper]QueryPage:" + sql);
            var items = Connection.Query(sql, map, parameters, DbTransaction, true, splitOn);
            return (count, items);
        }

        public async Task<(int count, IEnumerable<TEntity> items)> QueryPageAsync<TEntity>(int page, int size, dynamic where, dynamic order = null) where TEntity : class
        {
            (string sql, string sqlCount, DynamicParameters parameters) = _QueryPage(page, size, "*", Dialect.FormatTableName<TEntity>(), where as object, order as object);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]QueryPageAsync:" + sqlCount);
            var count = (await Connection.ExecuteScalarAsync<int?>(sqlCount, parameters, DbTransaction, Timeout)).GetValueOrDefault();
            if (count == 0 || count <= ((page - 1) * size)) return (count, new TEntity[] { });

            _logger.LogDebug("[Dapper]QueryPageAsync:" + sql);
            var items = await Connection.QueryAsync<TEntity>(sql, parameters, DbTransaction, Timeout);
            return (count, items);
        }

        public async Task<(int count, IEnumerable<TEntity> items)> QueryPageAsync<TEntity>(int page, int size, string feild, string tableJoin, dynamic where, dynamic order)
        {
            (string sql, string sqlCount, DynamicParameters parameters) = _QueryPage(page, size, feild, tableJoin, where as object, order as object);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]QueryPageAsync:" + sqlCount);
            var count = (await Connection.ExecuteScalarAsync<int?>(sqlCount, parameters, DbTransaction, Timeout)).GetValueOrDefault();
            if (count == 0 || count <= ((page - 1) * size)) return (count, new TEntity[] { });

            _logger.LogDebug("[Dapper]QueryPageAsync:" + sql);
            var items = await Connection.QueryAsync<TEntity>(sql, parameters, DbTransaction, Timeout);
            return (count, items);
        }

        public async Task<(int count, IEnumerable<TReturn> items)> QueryPageAsync<TFirst, TSecond, TReturn>(int page, int size, string feild, string tableJoin, dynamic where, dynamic order, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        {
            (string sql, string sqlCount, DynamicParameters parameters) = _QueryPage(page, size, feild, tableJoin, where as object, order as object);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]QueryPageAsync:" + sqlCount);
            var count = (await Connection.ExecuteScalarAsync<int?>(sqlCount, parameters, DbTransaction, Timeout)).GetValueOrDefault();
            if (count == 0 || count <= ((page - 1) * size)) return (count, new TReturn[] { });

            _logger.LogDebug("[Dapper]QueryPageAsync:" + sql);
            var items = await Connection.QueryAsync(sql, map, parameters, DbTransaction, true, splitOn);
            return (count, items);
        }

        private (string sql, string sqlCount, DynamicParameters parameters) _QueryPage(int page, int size, string feild, string tableJoin, object where, object order)
        {
            DynamicParameters parameters;

            //处理WHERE语句
            string whereSql;
            if (where is DapperSql dSql)
            {
                whereSql = Dialect.FormatWhereSql(null, dSql.Sql);
                parameters = new DynamicParameters(dSql.Parameters as object);
            }
            else if (where is string s)
            {
                whereSql = Dialect.FormatWhereSql(null, s);
                parameters = new DynamicParameters();
            }
            else
            {
                parameters = new DynamicParameters(where);
                var fields = GetDynamicFields(where).Select(x => x.Name).ToList();
                whereSql = Dialect.FormatWhereSql(fields, null);
            }

            //处理ORDER语句
            string orderSql = null;
            if (order is DapperSql dSqlOrder)
            {
                orderSql = Dialect.FormatOrderSql(null, dSqlOrder.Sql);
            }
            else if (order is string s)
            {
                orderSql = Dialect.FormatOrderSql(null, s);
            }
            else if (order != null)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                GetDynamicFields(order).ForEach(item => dict.Add(item.Name, (string)item.Value));
                orderSql = Dialect.FormatOrderSql(dict, null);
            }

            //取得总件数SQL
            var sqlCnt = $"SELECT COUNT(1) FROM {tableJoin}";
            sqlCnt += whereSql ?? "";

            var sql = Dialect.FormatQueryPageSql(page, size, $"SELECT {feild} FROM {tableJoin}{whereSql}{orderSql}");

            //var sql = Dialect.FormatQueryPageSql<TEntity>(page, size, "*", whereSql, orderSql);
            return (sql, sqlCnt, parameters);
        }

        #endregion

        #region 查询件数

        /// <summary>
        /// 查询件数
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>数据件数</returns>
        public int QueryCount<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _QueryCount<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]ExecuteScalar:" + sql);
            return Connection.ExecuteScalar<int?>(sql, parameters, DbTransaction, Timeout).GetValueOrDefault();
        }

        /// <summary>
        /// 查询件数
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>数据件数</returns>
        public async Task<int> QueryCountAsync<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _QueryCount<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                throw new DapperException("SQL异常！");
            }
            _logger.LogDebug("[Dapper]ExecuteScalarAsync:" + sql);
            return (await Connection.ExecuteScalarAsync<int?>(sql, parameters, DbTransaction, Timeout)).GetValueOrDefault();
        }

        private (string sql, DynamicParameters parameters) _QueryCount<TEntity>(object where) where TEntity : class
        {
            DynamicParameters parameters;
            string sql;

            if (where is DapperSql dSql)
            {
                sql = Dialect.FormatCountSql<TEntity>(null, dSql.Sql);
                parameters = new DynamicParameters(dSql.Parameters as object);
            }
            else
            {
                parameters = new DynamicParameters(where);
                var fields = GetDynamicFields(where).Select(x => x.Name).ToList();
                sql = Dialect.FormatCountSql<TEntity>(fields, null);
            }
            return (sql, parameters);
        }

        #endregion

        #region 插入数据

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns>操作数据条数</returns>
        public int Insert<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new DapperException("插入的数据不能为空！");
            }
            var fields = GetDynamicFields(entity).Select(x=>x.Name).ToList();
            var sql = Dialect.FormatInsertSql<TEntity>(fields);
            _logger.LogDebug("[Dapper]Execute:" + sql);
            return Connection.Execute(sql, entity, DbTransaction, Timeout);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns>操作数据条数</returns>
        public async Task<int> InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new DapperException("插入的数据不能为空！");
            }
            var fields = GetDynamicFields(entity).Select(x => x.Name).ToList();
            var sql = Dialect.FormatInsertSql<TEntity>(fields);
            _logger.LogDebug("[Dapper]ExecuteAsync:" + sql);
            return await Connection.ExecuteAsync(sql, entity, DbTransaction, Timeout);
        }

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <returns>操作数据条数</returns>
        public int Insert<TEntity>(IEnumerable<TEntity> entitys) where TEntity : class
        {
            if (entitys == null || !entitys.Any())
            {
                throw new DapperException("插入的数据不能为空！");
            }
            var fields = GetDynamicFields(entitys.First()).Select(x => x.Name).ToList();
            var sql = Dialect.FormatInsertSql<TEntity>(fields);
            _logger.LogDebug("[Dapper]Execute:" + sql);
            return Connection.Execute(sql, entitys, DbTransaction, Timeout);
        }

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <returns>操作数据条数</returns>
        public async Task<int> InsertAsync<TEntity>(IEnumerable<TEntity> entitys) where TEntity : class
        {
            if (entitys == null || !entitys.Any())
            {
                return 0;
            }
            var fields = GetDynamicFields(entitys.First()).Select(x => x.Name).ToList();
            var sql = Dialect.FormatInsertSql<TEntity>(fields);
            _logger.LogDebug("[Dapper]ExecuteAsync:" + sql);
            return await Connection.ExecuteAsync(sql, entitys, DbTransaction, Timeout);
        }

        #endregion

        #region 更新数据

        public int Update<TEntity>(dynamic data, dynamic where = null) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Update<TEntity>(data as object, where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            _logger.LogDebug("[Dapper]Execute:" + sql);
            return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        }

        public async Task<int> UpdateAsync<TEntity>(dynamic data, dynamic where = null) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Update<TEntity>(data as object, where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            _logger.LogDebug("[Dapper]ExecuteAsync:" + sql);
            return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        }

        private (string sql, DynamicParameters parameters) _Update<TEntity>(object data, object where) where TEntity : class
        {
            if (data == null) return (null, null);

            DynamicParameters parameters;
            var whereFields = new List<string>();
            var updateFields = new List<string>();

            //取得所有属性
            var dataFields = GetDynamicFields(data);
            updateFields = dataFields.Select(x => x.Name).ToList();

            if (where == null)
            {
                if (!(data is TEntity))
                {
                    return (null, null);
                }

                parameters = new DynamicParameters();

                foreach (var field in dataFields)
                {
                    if (field.IsKey)
                    {
                        //主键
                        parameters.Add("w_" + field.Name, field.Value);
                    }
                    else
                    {
                        parameters.Add(field.Name, field.Value); 
                    }
                }
            }
            else
            {
                var whereDynamicFields = GetDynamicFields(where);
                whereFields = whereDynamicFields.Select(x => x.Name).ToList();

                parameters = new DynamicParameters(data);
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                whereDynamicFields.ForEach(p => expandoObject.Add("w_" + p.Name, p.Value));
                parameters.AddDynamicParams(expandoObject);
            }

            var sql = Dialect.FormatUpdateSql(Dialect.FormatTableName<TEntity>(), updateFields, whereFields, "w_");

            return (sql, parameters);
        }

        //public int Update<TEntity>(TEntity entity) where TEntity : class
        //{
        //    return Update<TEntity>(entity, null as string[]);
        //}

        //public int Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] updateFileds) where TEntity : class
        //{
        //    string[] fileds = null;
        //    if (updateFileds != null)
        //    {
        //        fileds = updateFileds.Select(x => (x as MemberExpression)?.Member.Name).ToArray();
        //    }
        //    return Update<TEntity>(entity, fileds);
        //}

        //public int Update<TEntity>(TEntity entity, params string[] updateFileds) where TEntity : class
        //{
        //    if (entity == null)
        //    {
        //        return 0;
        //    }
        //    var parameters = new DynamicParameters();
        //    var whereFields = new List<string>();
        //    var updateFields = new List<string>();
        //    //取得所有属性
        //    var propertyInfos = GetPropertyInfos(entity);
        //    foreach (var property in propertyInfos)
        //    {
        //        if (property.GetCustomAttribute<KeyAttribute>() != null)
        //        {
        //            //主键
        //            parameters.Add("w_" + property.Name, property.GetValue(entity, null));

        //            whereFields.Add(property.Name);
        //        }
        //        else
        //        {
        //            if (updateFileds == null || updateFileds.Contains(property.Name))
        //            {
        //                parameters.Add(property.Name, property.GetValue(entity, null));
        //                updateFields.Add(property.Name);
        //            }
        //        }
        //    }

        //    var sql = Dialect.FormatUpdateSql<TEntity>(updateFields, whereFields, "w_");
        //    return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        //}

        //public int UpdateExt(string table, string tableSchema, dynamic data, dynamic where)
        //{
        //    var obj = data as object;
        //    var conditionObj = where as object;

        //    var wherePropertyInfos = GetPropertyInfos(conditionObj);

        //    var updateFields = GetProperties(obj);
        //    var whereFields = wherePropertyInfos.Select(x => x.Name).ToList();

        //    var sql = Dialect.FormatUpdateSql(table, tableSchema, updateFields, whereFields, "w_");

        //    var parameters = new DynamicParameters(obj);
        //    var expandoObject = new ExpandoObject() as IDictionary<string, object>;
        //    wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
        //    parameters.AddDynamicParams(expandoObject);

        //    return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        //}

        //public async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        //{
        //    return await UpdateAsync<TEntity>(entity, null as string[]);
        //}

        //public async Task<int> UpdateAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] updateFileds) where TEntity : class
        //{
        //    string[] fileds = null;
        //    if (updateFileds != null)
        //    {
        //        fileds = updateFileds.Select(x => (x as MemberExpression)?.Member.Name).ToArray();
        //    }
        //    return await UpdateAsync<TEntity>(entity, fileds);
        //}

        //public async Task<int> UpdateAsync<TEntity>(TEntity entity, params string[] updateFileds) where TEntity : class
        //{
        //    if (entity == null)
        //    {
        //        return 0;
        //    }
        //    var parameters = new DynamicParameters();
        //    var whereFields = new List<string>();
        //    var updateFields = new List<string>();
        //    //取得所有属性
        //    var propertyInfos = GetPropertyInfos(entity);
        //    foreach (var property in propertyInfos)
        //    {
        //        if (property.GetCustomAttribute<KeyAttribute>() != null)
        //        {
        //            //主键
        //            parameters.Add("w_" + property.Name, property.GetValue(entity, null));

        //            whereFields.Add(property.Name);
        //        }
        //        else
        //        {
        //            if (updateFileds == null || updateFileds.Contains(property.Name))
        //            {
        //                parameters.Add(property.Name, property.GetValue(entity, null));
        //                updateFields.Add(property.Name);
        //            }
        //        }
        //    }

        //    var sql = Dialect.FormatUpdateSql<TEntity>(updateFields, whereFields, "w_");
        //    return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        //}

        //public async Task<int> UpdateExtAsync(string table, string tableSchema, dynamic data, dynamic where)
        //{
        //    var obj = data as object;
        //    var conditionObj = where as object;

        //    var wherePropertyInfos = GetPropertyInfos(conditionObj);

        //    var updateFields = GetProperties(obj);
        //    var whereFields = wherePropertyInfos.Select(x => x.Name).ToList();

        //    var sql = Dialect.FormatUpdateSql(table, tableSchema, updateFields, whereFields, "w_");

        //    var parameters = new DynamicParameters(obj);
        //    var expandoObject = new ExpandoObject() as IDictionary<string, object>;
        //    wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
        //    parameters.AddDynamicParams(expandoObject);

        //    return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        //}

        #endregion

        #region 删除&逻辑删除

        public int Delete<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Delete<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            _logger.LogDebug("[Dapper]Execute:" + sql);
            return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        }

        public async Task<int> DeleteAsync<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Delete<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            _logger.LogDebug("[Dapper]ExecuteAsync:" + sql);
            return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        }

        private (string sql, DynamicParameters parameters) _Delete<TEntity>(object where) where TEntity : class
        {
            if (where == null) return (null, null);

            DynamicParameters parameters;
            string sql;

            var type = typeof(TEntity);
            var attr = type.GetCustomAttribute<LogicalDeleteAttribute>(true);
            if (attr == null)
            {
                //物理删除
                if (where is DapperSql dSql)
                {
                    sql = Dialect.FormatDeleteSql<TEntity>(null, dSql.Sql);
                    parameters = new DynamicParameters(dSql.Parameters as object);
                }
                else
                {
                    parameters = new DynamicParameters(where);
                    var fields = GetDynamicFields(where).Select(x => x.Name).ToList();

                    sql = Dialect.FormatDeleteSql<TEntity>(fields, null);
                }
            }
            else
            {
                if (where is DapperSql dSql)
                {
                    sql = Dialect.FormatLogicalDeleteRestoreSql<TEntity>(attr.Field, null, dSql.Sql);
                    parameters = new DynamicParameters(dSql.Parameters as object);
                    parameters.Add(attr.Field, attr.DeletedValue);
                }
                else
                {
                    parameters = new DynamicParameters(where);
                    var fields = GetDynamicFields(where).Select(x => x.Name).ToList();

                    sql = Dialect.FormatLogicalDeleteRestoreSql<TEntity>(attr.Field, fields);
                    parameters.Add(attr.Field, attr.DeletedValue);
                }
            }
            return (sql, parameters);
        }

        #endregion

        #region 逻辑恢复

        public int Restore<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Restore<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            _logger.LogDebug("[Dapper]Execute:" + sql);
            return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        }

        public async Task<int> RestoreAsync<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Restore<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            _logger.LogDebug("[Dapper]ExecuteAsync:" + sql);
            return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        }

        private (string sql, DynamicParameters parameters) _Restore<TEntity>(object where) where TEntity : class
        {
            if (where == null) return (null, null);

            var type = typeof(TEntity);
            var attr = type.GetCustomAttribute<LogicalDeleteAttribute>(true);
            if (attr == null)
            {
                throw new DapperException("该对象不支持逻辑恢复操作！");
            }

            DynamicParameters parameters;
            string sql;

            if (where is DapperSql dSql)
            {
                sql = Dialect.FormatLogicalDeleteRestoreSql<TEntity>(attr.Field, null, dSql.Sql);
                parameters = new DynamicParameters(dSql.Parameters as object);
                parameters.Add(attr.Field, attr.NormalValue);
            }
            else
            {
                parameters = new DynamicParameters(where);
                var fields = GetDynamicFields(where).Select(x => x.Name).ToList();

                sql = Dialect.FormatLogicalDeleteRestoreSql<TEntity>(attr.Field, fields);
                parameters.Add(attr.Field, attr.NormalValue);
            }
            return (sql, parameters);
        }

        #endregion

        private static readonly ConcurrentDictionary<string, IEnumerable<PropertyInfo>> _fieldCache = new ConcurrentDictionary<string, IEnumerable<PropertyInfo>>();

        internal static List<DapperDynamicField> GetDynamicFields(object obj)
        {
            if (obj == null)
            {
                return new List<DapperDynamicField>();
            }
            if (obj is DynamicParameters dp)
            {
                var fields = new List<DapperDynamicField>();
                foreach (var name in dp.ParameterNames)
                {
                    fields.Add(new DapperDynamicField {
                        Name = name,
                        Value = dp.Get<object>(name)
                    });
                }
                return fields;
            }
            if (obj is ExpandoObject eo)
            {
                var dic = (IDictionary<string, object>)eo;

                var fields = new List<DapperDynamicField>();
                foreach (var name in dic.Keys)
                {
                    fields.Add(new DapperDynamicField
                    {
                        Name = name,
                        Value = dic[name]
                    });
                }
                return fields;
            }
            if (obj.GetType().Name.Contains("AnonymousType"))
            {
                var props = obj.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
                var fields = new List<DapperDynamicField>();
                foreach (var p in props)
                {
                    fields.Add(new DapperDynamicField
                    {
                        Name = p.Name,
                        Value = p.GetValue(obj, null)
                    });
                }
                return fields;
            }

            var key = obj.GetType().FullName;
            IEnumerable<PropertyInfo> properties;
            if (!_fieldCache.TryGetValue(key, out properties))
            {
                properties = obj.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).Where(x => !x.GetAccessors()[0].IsVirtual && x.GetCustomAttribute<NotMappedAttribute>() == null);
                _fieldCache.TryAdd(key, properties);
            }

            var fieldlst = new List<DapperDynamicField>();
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    fieldlst.Add(new DapperDynamicField
                    {
                        Name = p.Name,
                        Value = p.GetValue(obj, null),
                        IsKey = p.GetCustomAttribute<KeyAttribute>() != null
                    });
                }
            }
            return fieldlst;
        }

        #region Dispose

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            _transaction = null;

            if (Connection != null)
            {
                Connection.Dispose();
            }
            Connection = null;
        }

        #endregion

        public void Log(string method, string message)
        {
            _logger.LogDebug($"[Dapper]{method}:{message}");
        }
    }
}
