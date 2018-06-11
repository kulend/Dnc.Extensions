using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Ku.Core.Extensions.DbMigration.MySql
{
    public class MySqlDbTool : IDbTool
    {
        private readonly DbMigrationOptions _options;

        private IEnumerable<DbTableInfo> Tables;

        public MySqlDbTool(IOptions<DbMigrationOptions> option)
        {
            _options = option.Value;
        }

        public IEnumerable<DbTableInfo> GetTables()
        {
            return Tables;
        }

        public async Task<IEnumerable<DbTableInfo>> GetTablesAsync()
        {
            IDbConnection connection = new MySqlConnection(_options.ConnectionString);
            var sql = "SELECT TABLE_NAME as TableName, " +
                "table_comment AS Comment " +
                "from information_schema.tables " +
                $"WHERE table_schema='{_options.DataBaseSchema}' and table_type='BASE TABLE' ";

            Tables =  await connection.QueryAsync<DbTableInfo>(sql);
            return Tables;
        }

        /// <summary>
        /// 取得表字段信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DbFieldInfo>> GetTableFieldsAsync(string tableName)
        {
            IDbConnection connection = new MySqlConnection(_options.ConnectionString);
            var sql = "SELECT TABLE_NAME as TableName, " +
                "column_name AS FieldName,  " +
                "data_type AS DataType,  " +
                "character_maximum_length AS Length, " +
                "numeric_precision AS NumLength, " +
                "numeric_scale AS Scale, " +
                "datetime_precision AS DatetimePrecision, " +
                "CASE WHEN is_nullable = 'YES' THEN 1 ELSE 0 END AS Nullable, " +
                "CASE WHEN extra = 'auto_increment' THEN 1 ELSE 0 END AS Increment, " +
                "column_default AS DefValue, " +
                "CASE WHEN column_key = 'PRI' THEN 1 ELSE 0 END AS IsKey, " +
                "column_comment AS Comment " +
                "from information_schema.COLUMNS " +
                $"WHERE TABLE_NAME='{tableName}'";

            
            var fields = await connection.QueryAsync<MySqlFieldInfo>(sql);

            return fields.Select(x => new DbFieldInfo
            {
                Name = x.FieldName,
                DataType = FormatFieldDataType(x),
                Nullable = x.Nullable,
                AutoIncrement = x.Increment,
                IsKey = x.IsKey,
                Comment = x.Comment
            });
        }

        private string FormatFieldDataType(MySqlFieldInfo field)
        {
            if (field.DataType.Equals("varchar"))
            {
                return $"varchar({field.Length})";
            }
            else if (field.DataType.Equals("char"))
            {
                return $"char({field.Length})";
            }
            else if (field.DataType.Equals("datetime"))
            {
                return $"datetime({field.DatetimePrecision})";
            }
            else
            {
                return field.DataType;
            }
        }
    }

    public class MySqlFieldInfo
    {
        public string TableName { set; get; }

        public string FieldName { set; get; }

        public string DataType { set; get; }

        public long? Length { set; get; }

        public long? NumLength { set; get; }

        public long? Scale { set; get; }

        public long? DatetimePrecision { set; get; }

        public bool Nullable { set; get; }

        public bool Increment { set; get; }

        public string DefValue { set; get; }

        public bool IsKey { set; get; }

        public string Comment { set; get; }
    }
}
