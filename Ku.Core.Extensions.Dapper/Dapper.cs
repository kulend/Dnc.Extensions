using Dapper;
using Ku.Core.Extensions.Dapper.Attributes;
using Ku.Core.Extensions.Dapper.Sql;
using Ku.Core.Extensions.Dapper.SqlDialect;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.Dapper
{
    internal class Dapper : IDapper
    {
        private DapperOptions _options;
        private ITransation _transaction;
        private IDbTransaction DbTransaction { get { return _transaction?.Transaction; } }

        public int? Timeout { set; get; } = null;

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public IDbConnection Connection { get; private set; }

        /// <summary>
        /// Sql语法工具
        /// </summary>
        public ISqlDialect Dialect { set; get; }

        public Dapper(IOptions<DapperOptions> options)
        {
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

        public TEntity QueryOne<TEntity>(dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(conditionObj);
            var whereFields = GetProperties(conditionObj);
            var sql = Dialect.FormatQuerySql<TEntity>(null, whereFields, order, true);

            return Connection.QueryFirstOrDefault<TEntity>(sql, parameters, DbTransaction, Timeout);
        }

        public async Task<TEntity> QueryOneAsync<TEntity>(dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(conditionObj);
            var whereFields = GetProperties(conditionObj);
            var sql = Dialect.FormatQuerySql<TEntity>(null, whereFields, order, true);

            return await Connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, DbTransaction, Timeout);
        }

        public IEnumerable<TEntity> QueryList<TEntity>(dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(conditionObj);
            var whereFields = GetProperties(conditionObj);
            var sql = Dialect.FormatQuerySql<TEntity>(null, whereFields, order, false);

            return Connection.Query<TEntity>(sql, parameters, DbTransaction, true, Timeout);
        }

        public async Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(conditionObj);
            var whereFields = GetProperties(conditionObj);
            var sql = Dialect.FormatQuerySql<TEntity>(null, whereFields, order, false);

            return await Connection.QueryAsync<TEntity>(sql, parameters, DbTransaction, Timeout);
        }

        public (int count, IEnumerable<TEntity> items) QueryPage<TEntity>(int page, int size, dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(conditionObj);
            var whereFields = GetProperties(conditionObj);

            var sql = Dialect.FormatCountSql<TEntity>(whereFields);
            //取得总件数
            var count = Connection.ExecuteScalar<int?>(sql, parameters, DbTransaction, Timeout).GetValueOrDefault();
            if (count == 0)
            {
                return (0, new TEntity[] { });
            }

            if (count <= ((page - 1) * size))
            {
                return (count, new TEntity[] { });
            }

            var sql2 = Dialect.FormatQueryPageSql<TEntity>(page, size, null, whereFields, order);

            var items = Connection.Query<TEntity>(sql, parameters, DbTransaction, true, Timeout);
            return (count, items);
        }

        public async Task<(int count, IEnumerable<TEntity> items)> QueryPageAsync<TEntity>(int page, int size, dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(conditionObj);
            var whereFields = GetProperties(conditionObj);

            var sql = Dialect.FormatCountSql<TEntity>(whereFields);
            //取得总件数
            var count = (await Connection.ExecuteScalarAsync<int?>(sql, parameters, DbTransaction, Timeout)).GetValueOrDefault();
            if (count == 0)
            {
                return (0, new TEntity[] { });
            }

            if (count <= ((page - 1) * size))
            {
                return (count, new TEntity[] { });
            }

            var sql2 = Dialect.FormatQueryPageSql<TEntity>(page, size, null, whereFields, order);

            var items = await Connection.QueryAsync<TEntity>(sql, parameters, DbTransaction, Timeout);
            return (count, items);
        }

        #endregion

        #region 件数

        public int QueryCount<TEntity>(dynamic where) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(conditionObj);
            var whereFields = GetProperties(conditionObj);
            var sql = Dialect.FormatCountSql<TEntity>(whereFields);

            return Connection.ExecuteScalar<int?>(sql, parameters, DbTransaction, Timeout).GetValueOrDefault();
        }

        public async Task<int> QueryCountAsync<TEntity>(dynamic where) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(conditionObj);
            var whereFields = GetProperties(conditionObj);
            var sql = Dialect.FormatCountSql<TEntity>(whereFields);

            return (await Connection.ExecuteScalarAsync<int?>(sql, parameters, DbTransaction, Timeout)).GetValueOrDefault();
        }

        #endregion

        #region 插入数据

        public bool Insert<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return false;
            }
            var fields = GetProperties(entity);
            var sql = Dialect.FormatInsertSql<TEntity>(fields);
            return Connection.Execute(sql, entity, DbTransaction, Timeout) > 0;
        }

        public async Task<bool> InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return false;
            }
            var fields = GetProperties(entity);
            var sql = Dialect.FormatInsertSql<TEntity>(fields);
            return await Connection.ExecuteAsync(sql, entity, DbTransaction, Timeout) > 0;
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

            return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        }

        public async Task<int> UpdateAsync<TEntity>(dynamic data, dynamic where = null) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Update<TEntity>(data as object, where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }

            return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        }

        private (string sql, DynamicParameters parameters) _Update<TEntity>(object data, object where) where TEntity : class
        {
            if (data == null) return (null, null);

            DynamicParameters parameters;
            var whereFields = new List<string>();
            var updateFields = new List<string>();

            //取得所有属性
            var propertyInfos = GetPropertyInfos(data);
            if (where == null)
            {
                if (!(data is TEntity))
                {
                    return (null, null);
                }

                parameters = new DynamicParameters();

                foreach (var property in propertyInfos)
                {
                    if (property.GetCustomAttribute<KeyAttribute>() != null)
                    {
                        //主键
                        parameters.Add("w_" + property.Name, property.GetValue(data, null));

                        whereFields.Add(property.Name);
                    }
                    else
                    {
                        parameters.Add(property.Name, property.GetValue(data, null));
                        updateFields.Add(property.Name);
                    }
                }
            }
            else
            {
                updateFields = propertyInfos.Select(x => x.Name).ToList();

                var wherePropertyInfos = GetPropertyInfos(where);
                whereFields = wherePropertyInfos.Select(x => x.Name).ToList();

                parameters = new DynamicParameters(data);
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(where, null)));
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

        public int UpdateExt(string table, string tableSchema, dynamic data, dynamic where)
        {
            var obj = data as object;
            var conditionObj = where as object;

            var wherePropertyInfos = GetPropertyInfos(conditionObj);

            var updateFields = GetProperties(obj);
            var whereFields = wherePropertyInfos.Select(x => x.Name).ToList();

            var sql = Dialect.FormatUpdateSql(table, tableSchema, updateFields, whereFields, "w_");

            var parameters = new DynamicParameters(obj);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
            parameters.AddDynamicParams(expandoObject);

            return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        }

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

        public async Task<int> UpdateExtAsync(string table, string tableSchema, dynamic data, dynamic where)
        {
            var obj = data as object;
            var conditionObj = where as object;

            var wherePropertyInfos = GetPropertyInfos(conditionObj);

            var updateFields = GetProperties(obj);
            var whereFields = wherePropertyInfos.Select(x => x.Name).ToList();

            var sql = Dialect.FormatUpdateSql(table, tableSchema, updateFields, whereFields, "w_");

            var parameters = new DynamicParameters(obj);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
            parameters.AddDynamicParams(expandoObject);

            return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        }

        #endregion

        #region 删除

        public int Delete<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Delete<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        }

        public async Task<int> DeleteAsync<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Delete<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        }

        private (string sql, DynamicParameters parameters) _Delete<TEntity>(object where) where TEntity : class
        {
            if (where == null) return (null, null);

            var type = typeof(TEntity);
            var attr = type.GetCustomAttribute<LogicalDeleteAttribute>(true);
            if (attr == null)
            {
                return (null, null);
            }

            DynamicParameters parameters;
            string sql;

            if (where is DapperSql dSql)
            {
                sql = Dialect.FormatLogicalDeleteRestoreSql<TEntity>(attr.Field, null, dSql.Sql);
                parameters = new DynamicParameters(dSql.Parameters as object);
                parameters.Add(attr.Field, attr.DeletedValue);
            }
            else
            {
                parameters = new DynamicParameters(where);
                var fields = GetProperties(where);

                sql = Dialect.FormatLogicalDeleteRestoreSql<TEntity>(attr.Field, fields);
                parameters.Add(attr.Field, attr.DeletedValue);
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
            return Connection.Execute(sql, parameters, DbTransaction, Timeout);
        }

        public async Task<int> RestoreAsync<TEntity>(dynamic where) where TEntity : class
        {
            (string sql, DynamicParameters parameters) = _Restore<TEntity>(where as object);
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            return await Connection.ExecuteAsync(sql, parameters, DbTransaction, Timeout);
        }

        private (string sql, DynamicParameters parameters) _Restore<TEntity>(object where) where TEntity : class
        {
            if (where == null) return (null, null);

            var type = typeof(TEntity);
            var attr = type.GetCustomAttribute<LogicalDeleteAttribute>(true);
            if (attr == null)
            {
                return (null, null);
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
                var fields = GetProperties(where);

                sql = Dialect.FormatLogicalDeleteRestoreSql<TEntity>(attr.Field, fields);
                parameters.Add(attr.Field, attr.NormalValue);
            }
            return (sql, parameters);
        }

        #endregion

        private static readonly ConcurrentDictionary<string, List<PropertyInfo>> _paramCache = new ConcurrentDictionary<string, List<PropertyInfo>>();

        private static List<string> GetProperties(object obj)
        {
            if (obj == null)
            {
                return new List<string>();
            }
            if (obj is DynamicParameters)
            {
                return (obj as DynamicParameters).ParameterNames.ToList();
            }
            return GetPropertyInfos(obj).Select(x => x.Name).ToList();
        }

        private static List<PropertyInfo> GetPropertyInfos(object obj)
        {
            if (obj == null)
            {
                return new List<PropertyInfo>();
            }

            List<PropertyInfo> properties;
            if (_paramCache.TryGetValue(obj.GetType().FullName, out properties)) return properties.ToList();
            properties = obj.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).Where(x=>!x.GetAccessors()[0].IsVirtual) .ToList();
            _paramCache[obj.GetType().FullName] = properties;
            return properties;
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
    }
}
