using MySql.Data.MySqlClient;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DapperExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, Action<Ku.Core.Extensions.Dapper.DapperOptions> options)
        {
            services.Configure(options);
            services.AddScoped<Ku.Core.Extensions.Dapper.IDapper, Ku.Core.Extensions.Dapper.Dapper>();
            return services;
        }
    }

    public static class MySqlDapperOptionsExtensions
    {
        public static Ku.Core.Extensions.Dapper.DapperOptions UseMySql(this Ku.Core.Extensions.Dapper.DapperOptions options, string connectionString)
        {
            options.DbType = Ku.Core.Extensions.Dapper.DbType.MySql;
            options.SqlDialect = new Ku.Core.Extensions.Dapper.SqlDialect.MySqlDialect();
            options.DbConnection = () => new MySqlConnection(connectionString);
            return options;
        }
    }
}

