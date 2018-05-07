using Dapper;
using Ku.Core.Extensions.Dapper.Sql;
using Ku.Core.Extensions.Dapper.SqlDialect;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
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
        #region 主键查询

        public TEntity QueryById<TEntity>(object id) where TEntity : class
        {
            var pms = new Dictionary<string, object>();
            var sss = ExpressionHelper.GetExpressionSql<User>(x=>x.AA.Equals("A") && x.BB == 1, ref pms);
            var sql = $"SELECT * FROM {_dialect.GetTableNameWithSchema<TEntity>()} WHERE Id=@Id";
            return _conn.QueryFirstOrDefault<TEntity>(sql, new { Id = id });
        }

        public async Task<TEntity> QueryByIdAsync<TEntity>(object id) where TEntity : class
        {
            var sql = $"SELECT * FROM {_dialect.GetTableNameWithSchema<TEntity>()} WHERE Id=@Id";
            return await _conn.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = id });
        }

        #endregion

        #region 通用查询

        public TEntity PageQuery<TEntity>(object id) where TEntity : class
        {
            var sql = $"SELECT * FROM {_dialect.GetTableNameWithSchema<TEntity>()} WHERE Id=@Id";
            return null;
        }

        #endregion

        public class User
        {
            public string AA { set; get; }

            public int BB { set; get; }

            public bool CC { set; get; }
        }
    }
}
