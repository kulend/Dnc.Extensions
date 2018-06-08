using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.DbMigration.MySql
{
    public class MySqlDbTool : IDbTool
    {
        private readonly DbMigrationOptions _options;

        public MySqlDbTool(IOptions<DbMigrationOptions> option)
        {
            _options = option.Value;
        }

        public async Task<IEnumerable<DbFieldInfo>> GetTableFieldsAsync(string tableName)
        {
            IDbConnection connection = new MySqlConnection(_options.ConnectionString);
            var sql = "SELECT TABLE_NAME as TableName, " +
                "column_name AS FieldName,  " +
                "data_type AS DataType,  " +
                "character_maximum_length AS Length, " +
                "numeric_precision AS NumLength, " +
                "numeric_scale AS Scale, " +
                "CASE WHEN is_nullable = 'YES' THEN 1 ELSE 0 END AS Nullable, " +
                "CASE WHEN extra = 'auto_increment' THEN 1 ELSE 0 END AS Increment, " +
                "column_default AS DefValue, " +
                "column_comment AS Comment " +
                "from information_schema.COLUMNS " +
                $"WHERE TABLE_NAME='{tableName}'";

            
            return await connection.QueryAsync<DbFieldInfo>(sql);
        }
    }
}
