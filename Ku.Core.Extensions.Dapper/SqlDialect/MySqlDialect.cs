using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.Dapper.SqlDialect
{
    public class MySqlDialect : BaseDialect
    {
        public override char QuoteStart
        {
            get { return '`'; }
        }

        public override char QuoteEnd
        {
            get { return '`'; }
        }

        /// <summary>
        /// 分页查询语句
        /// </summary>
        public override string GetPageQuerySql<TEntity>(int page, int rows, string orderBy)
        {
            var tableName = GetTableNameWithSchema<TEntity>();
            //SELECT * FROM table ORDER BY id LIMIT 1000,10;
            return $"SELECT * FROM {tableName} ORDER BY {orderBy} LIMIT 1000,10";
        }
    }
}
