using Ku.Core.Extensions.DbMigration;
using Ku.Core.Extensions.DbMigration.MySql;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MySqlExtensions
    {
        public static void UseMySql(this DbMigrationOptions self, string connection)
        {
            self.ConnectionString = connection;
        }

        public static IServiceCollection AddDbMigration(this IServiceCollection self, Action<DbMigrationOptions> options = null)
        {
            DbMigrationExtensions.ConfigureServices(self, options);

            self.TryAddSingleton<IDbTool, MySqlDbTool>();

            return self;
        }
    }
}
