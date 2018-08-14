using Dapper;
using Dnc.Extensions.Dapper.Attributes;
using Dnc.Extensions.Dapper.Builders;
using Dnc.Extensions.Dapper.SqlDialect;
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

namespace Dnc.Extensions.Dapper
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
