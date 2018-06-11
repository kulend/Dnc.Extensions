using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.DbMigration
{
    public interface IDbTool
    {
        IEnumerable<DbTableInfo> GetTables();

        Task<IEnumerable<DbTableInfo>> GetTablesAsync();

        Task<IEnumerable<DbFieldInfo>> GetTableFieldsAsync(string tableName);
    }
}
