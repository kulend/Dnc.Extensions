using Dapper;
using Ku.Core.Extensions.Dapper.Attributes;
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
    public class Dapper : IDapper
    {
        private DapperOptions _options;
        private IDbConnection _conn;

        /// <summary>
        /// Sql语法工具
        /// </summary>
        private ISqlDialect _dialect;

        public Dapper(IOptions<DapperOptions> options)
        {
            this._options = options.Value;
            this._conn = _options.DbConnection();
            this._dialect = _options.SqlDialect;
        }

        #region 查询

        public TEntity QueryOne<TEntity>(dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(where);
            var whereFields = GetProperties(conditionObj);
            var sql = _dialect.FormatQuerySql<TEntity>(null, whereFields, order, true);

            return _conn.QueryFirstOrDefault<TEntity>(sql, parameters);
        }

        public async Task<TEntity> QueryOneAsync<TEntity>(dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(where);
            var whereFields = GetProperties(conditionObj);
            var sql = _dialect.FormatQuerySql<TEntity>(null, whereFields, order, true);

            return await _conn.QueryFirstOrDefaultAsync<TEntity>(sql, parameters);
        }

        public IEnumerable<TEntity> QueryList<TEntity>(dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(where);
            var whereFields = GetProperties(conditionObj);
            var sql = _dialect.FormatQuerySql<TEntity>(null, whereFields, order, false);

            return _conn.Query<TEntity>(sql, parameters);
        }

        public async Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(where);
            var whereFields = GetProperties(conditionObj);
            var sql = _dialect.FormatQuerySql<TEntity>(null, whereFields, order, false);

            return await _conn.QueryAsync<TEntity>(sql, parameters);
        }

        public (int count, IEnumerable<TEntity> items) QueryPage<TEntity>(int page, int size, dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(where);
            var whereFields = GetProperties(conditionObj);

            var sql = _dialect.FormatCountSql<TEntity>(whereFields);
            //取得总件数
            var count = _conn.ExecuteScalar<int?>(sql, parameters).GetValueOrDefault();
            if (count == 0)
            {
                return (0, new TEntity[] { });
            }

            if (count <= ((page - 1) * size))
            {
                return (count, new TEntity[] { });
            }

            var sql2 = _dialect.FormatQueryPageSql<TEntity>(page, size, null, whereFields, order);

            var items = _conn.Query<TEntity>(sql, parameters);
            return (count, items);
        }

        public async Task<(int count, IEnumerable<TEntity> items)> QueryPageAsync<TEntity>(int page, int size, dynamic where, string order = null) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(where);
            var whereFields = GetProperties(conditionObj);

            var sql = _dialect.FormatCountSql<TEntity>(whereFields);
            //取得总件数
            var count = (await _conn.ExecuteScalarAsync<int?>(sql, parameters)).GetValueOrDefault();
            if (count == 0)
            {
                return (0, new TEntity[] { });
            }

            if (count <= ((page - 1) * size))
            {
                return (count, new TEntity[] { });
            }

            var sql2 = _dialect.FormatQueryPageSql<TEntity>(page, size, null, whereFields, order);

            var items = await _conn.QueryAsync<TEntity>(sql, parameters);
            return (count, items);
        }

        #endregion

        #region 件数

        public int QueryCount<TEntity>(dynamic where) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(where);
            var whereFields = GetProperties(conditionObj);
            var sql = _dialect.FormatCountSql<TEntity>(whereFields);

            return _conn.ExecuteScalar<int?>(sql, parameters).GetValueOrDefault();
        }

        public async Task<int> QueryCountAsync<TEntity>(dynamic where) where TEntity : class
        {
            var conditionObj = where as object;
            var parameters = new DynamicParameters(where);
            var whereFields = GetProperties(conditionObj);
            var sql = _dialect.FormatCountSql<TEntity>(whereFields);

            return (await _conn.ExecuteScalarAsync<int?>(sql, parameters)).GetValueOrDefault();
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
            var sql = _dialect.FormatInsertSql<TEntity>(fields);
            return _conn.Execute(sql, entity) > 0;
        }

        public async Task<bool> InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return false;
            }
            var fields = GetProperties(entity);
            var sql = _dialect.FormatInsertSql<TEntity>(fields);
            return await _conn.ExecuteAsync(sql, entity) > 0;
        }

        #endregion

        #region 更新数据

        public int Update<TEntity>(TEntity entity) where TEntity : class
        {
            return Update<TEntity>(entity, null as string[]);
        }

        public int Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] updateFileds) where TEntity : class
        {
            string[] fileds = null;
            if (updateFileds != null)
            {
                fileds = updateFileds.Select(x => (x as MemberExpression)?.Member.Name).ToArray();
            }
            return Update<TEntity>(entity, fileds);
        }

        public int Update<TEntity>(TEntity entity, params string[] updateFileds) where TEntity : class
        {
            if (entity == null)
            {
                return 0;
            }
            var parameters = new DynamicParameters();
            var whereFields = new List<string>();
            var updateFields = new List<string>();
            //取得所有属性
            var propertyInfos = GetPropertyInfos(entity);
            foreach (var property in propertyInfos)
            {
                if (property.GetCustomAttribute<KeyAttribute>() != null)
                {
                    //主键
                    parameters.Add("w_" + property.Name, property.GetValue(entity, null));

                    whereFields.Add(property.Name);
                }
                else
                {
                    if (updateFileds == null || updateFileds.Contains(property.Name))
                    {
                        parameters.Add(property.Name, property.GetValue(entity, null));
                        updateFields.Add(property.Name);
                    }
                }
            }

            var sql = _dialect.FormatUpdateSql<TEntity>(updateFields, whereFields, "w_");
            return _conn.Execute(sql, parameters);
        }

        public int UpdateExt(string table, string tableSchema, dynamic data, dynamic condition)
        {
            var obj = data as object;
            var conditionObj = condition as object;

            var wherePropertyInfos = GetPropertyInfos(conditionObj);

            var updateFields = GetProperties(obj);
            var whereFields = wherePropertyInfos.Select(x => x.Name).ToList();

            var sql = _dialect.FormatUpdateSql(table, tableSchema, updateFields, whereFields, "w_");

            var parameters = new DynamicParameters(data);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
            parameters.AddDynamicParams(expandoObject);

            return _conn.Execute(sql, parameters);
        }

        public async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await UpdateAsync<TEntity>(entity, null as string[]);
        }

        public async Task<int> UpdateAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] updateFileds) where TEntity : class
        {
            string[] fileds = null;
            if (updateFileds != null)
            {
                fileds = updateFileds.Select(x => (x as MemberExpression)?.Member.Name).ToArray();
            }
            return await UpdateAsync<TEntity>(entity, fileds);
        }

        public async Task<int> UpdateAsync<TEntity>(TEntity entity, params string[] updateFileds) where TEntity : class
        {
            if (entity == null)
            {
                return 0;
            }
            var parameters = new DynamicParameters();
            var whereFields = new List<string>();
            var updateFields = new List<string>();
            //取得所有属性
            var propertyInfos = GetPropertyInfos(entity);
            foreach (var property in propertyInfos)
            {
                if (property.GetCustomAttribute<KeyAttribute>() != null)
                {
                    //主键
                    parameters.Add("w_" + property.Name, property.GetValue(entity, null));

                    whereFields.Add(property.Name);
                }
                else
                {
                    if (updateFileds == null || updateFileds.Contains(property.Name))
                    {
                        parameters.Add(property.Name, property.GetValue(entity, null));
                        updateFields.Add(property.Name);
                    }
                }
            }

            var sql = _dialect.FormatUpdateSql<TEntity>(updateFields, whereFields, "w_");
            return await _conn.ExecuteAsync(sql, parameters);
        }

        public async Task<int> UpdateExtAsync(string table, string tableSchema, dynamic data, dynamic condition)
        {
            var obj = data as object;
            var conditionObj = condition as object;

            var wherePropertyInfos = GetPropertyInfos(conditionObj);

            var updateFields = GetProperties(obj);
            var whereFields = wherePropertyInfos.Select(x => x.Name).ToList();

            var sql = _dialect.FormatUpdateSql(table, tableSchema, updateFields, whereFields, "w_");

            var parameters = new DynamicParameters(data);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
            parameters.AddDynamicParams(expandoObject);

            return await _conn.ExecuteAsync(sql, parameters);
        }

        #endregion

        #region 删除

        public int Delete<TEntity>(dynamic condition) where TEntity : class
        {
            if (condition == null) return 0;
            var type = typeof(TEntity);
            var attr = type.GetCustomAttribute<LogicalDeleteAttribute>(true);

            var conditionObj = condition as object;
            var parameters = new DynamicParameters(condition);
            var fields = GetProperties(conditionObj);

            var sql = "";
            if (attr == null)
            {
                //实际删除
                sql = _dialect.FormatDeleteSql<TEntity>(fields);
            }
            else
            {
                //逻辑删除
                sql = _dialect.FormatLogicalDeleteSql<TEntity>(attr.Field, fields);
                parameters.Add(attr.Field, attr.DeletedValue);
            }

            return _conn.Execute(sql, parameters);
        }

        public async Task<int> DeleteAsync<TEntity>(dynamic condition) where TEntity : class
        {
            if (condition == null) return 0;
            var type = typeof(TEntity);
            var attr = type.GetCustomAttribute<LogicalDeleteAttribute>(true);

            var conditionObj = condition as object;
            var parameters = new DynamicParameters(condition);
            var fields = GetProperties(conditionObj);

            var sql = "";
            if (attr == null)
            {
                //实际删除
                sql = _dialect.FormatDeleteSql<TEntity>(fields);
            }
            else
            {
                //逻辑删除
                sql = _dialect.FormatLogicalDeleteSql<TEntity>(attr.Field, fields);
                parameters.Add(attr.Field, attr.DeletedValue);
            }

            return await _conn.ExecuteAsync(sql, parameters);
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
            properties = obj.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).ToList();
            _paramCache[obj.GetType().FullName] = properties;
            return properties;
        }

    }
}
